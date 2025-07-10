// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2025 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using BadEcho.Properties;

namespace BadEcho.Interop.Dialogs;

/// <summary>
/// Provides a custom marshaller for task dialog configurations.
/// </summary>
[CustomMarshaller(typeof(TaskDialogConfiguration), MarshalMode.ManagedToUnmanagedRef, typeof(ManagedToUnmanagedRef))]
internal static unsafe class TaskDialogConfigurationMarshaller
{
    /// <summary>
    /// Represents a stateful marshaller for task dialog configurations.
    /// </summary>
    /// <remarks>
    /// We use a stateful marshaller since the unmanaged memory allocated to store the output needs to be tracked separate
    /// from said output. See <see cref="ToUnmanaged"/> for more information.
    /// </remarks>
    public ref struct ManagedToUnmanagedRef
    {
        private TASKDIALOGCONFIG _unmanaged;
        private TaskDialogConfiguration _managed;
        private IntPtr _pTaskConfig;
        
        private WindowHandle? _ownerHandle;
        private bool _ownerHandleAddRefd;

        /// <summary>
        /// Converts a managed <see cref="TaskDialogConfiguration"/> instance to its unmanaged counterpart,
        /// loading the result into the marshaller.
        /// </summary>
        /// <param name="managed">A managed instance of a task dialog configuration.</param>
        /// <remarks>
        /// <para>
        /// The task dialog configuration structure is more complex than the usual C-style struct we deal with when using P/Invoke.
        /// It contains numerous string pointers as well as a few pointers to several arrays of structs defining the input controls.
        /// Marshalling this structure using the traditional approach would require many allocations on the heap and lots of unmanaged
        /// resources to keep track of and clean up.
        /// </para>
        /// <para>
        /// For our marshalling strategy, we take the novel (at least in the land of managed code) and interesting approach kpreisser
        /// used in his Task Dialog interop code that would eventually be merged into the Windows Forms library. A single block of
        /// contiguous memory is allocated such that the configuration structure, its strings, and input control arrays can fit within it.
        /// </para>
        /// <para>
        /// This means we only have to do a single allocation and deallocation, which is good for performance and also keeps everything
        /// nice and tidy. The downside is that we have to take care in calculating our required memory size and engage in some pointer
        /// arithmetic. Which sounds like fun to me!
        /// </para>
        /// </remarks>
        public void FromManaged(TaskDialogConfiguration managed)
        {
            Require.NotNull(managed, nameof(managed));
            
            if (managed.Host == null)
                throw new InvalidOperationException();

            TaskDialog host = managed.Host;
            TaskDialogButtonFlags standardButtons = default;
            var customButtons = new List<TaskDialogButton>();

            foreach (var button in managed.PushButtons)
            {
                if (!button.IsStandard)
                {
                    customButtons.Add(button);
                    continue;
                }

                standardButtons |= button.GetStandardButtonFlag();
            }

            // A byte pointer is used for our allocation size since pointer arithmetic is used to calculate it.
            byte* sizeToAllocate 
                = CalculateSizeToAllocate(managed, customButtons);

            _pTaskConfig = Marshal.AllocHGlobal((IntPtr) (sizeToAllocate + (IntPtr.Size - 1)));

            try
            {   // Objects must be created in the same order with the same alignment used during size calculation.
                // Refer to CalculateSizeToAllocate for explanations behind each alignment.
                var pCurrent = (byte*) _pTaskConfig;
                
                // Because our allocated size pointer started on an aligned address (0), we want to make sure our pointer starts off
                // aligned as well. The rest of the alignment done mirror the ones done in CalculateSizeToAllocate.
                Align(ref pCurrent);
                
                // Declare a ref local pointed to the start of our allocated block. This is where the config struct will live.
                ref TASKDIALOGCONFIG unmanaged = ref *(TASKDIALOGCONFIG*) pCurrent;
                
                pCurrent += sizeof(TASKDIALOGCONFIG);

                Align(ref pCurrent, sizeof(char));
                
                TASKDIALOGCONFIG.IconUnion mainIcon = default;
                TASKDIALOGCONFIG.IconUnion footerIcon = default;

                mainIcon.pszIcon = (char*) (ushort) managed.MainIcon;
                footerIcon.pszIcon = (char*) (ushort) managed.FooterIcon;

                unmanaged = new TASKDIALOGCONFIG
                            {
                                cbSize = (uint) sizeof(TASKDIALOGCONFIG),
                                dwFlags = managed.Flags,
                                dwCommonButtons = standardButtons,
                                mainIcon = mainIcon,
                                footerIcon = footerIcon,
                                pszWindowTitle = MarshalString(managed.Title),
                                pszMainInstruction = MarshalString(managed.InstructionText),
                                pszContent = MarshalString(managed.Text),
                                pszVerificationText = MarshalString(managed.VerificationText),
                                pszExpandedInformation = MarshalString(managed.ExpandedText),
                                pszExpandedControlText = MarshalString(managed.ExpandedLabel),
                                pszCollapsedControlText = MarshalString(managed.CollapsedLabel),
                                pszFooter = MarshalString(managed.FooterText),
                                nDefaultButton = managed.DefaultButton?.Id ?? 0,
                                nDefaultRadioButton = managed.DefaultRadioButton?.Id ?? 0,
                                pfCallback = &NativeCallbackProc,
                                lpCallbackData = host.Handle
                            };

                if (managed.Owner != null)
                {
                    _ownerHandle = managed.Owner;
                    _ownerHandle.DangerousAddRef(ref _ownerHandleAddRefd);

                    unmanaged.hwndParent = _ownerHandle.DangerousGetHandle();
                }

                if (customButtons.Count > 0)
                {
                    Align(ref pCurrent);
                    var pButtons = (TASKDIALOG_BUTTON*) pCurrent;
                    
                    unmanaged.pButtons = pButtons;
                    unmanaged.cButtons = (uint) customButtons.Count;

                    pCurrent += sizeof(TASKDIALOG_BUTTON) * customButtons.Count;

                    Align(ref pCurrent, sizeof(char));

                    int i = 0;

                    foreach (TaskDialogButton customButton in customButtons)
                    {
                        pButtons[i++] = new TASKDIALOG_BUTTON
                                      {
                                          nButtonID = customButton.Id,
                                          pszButtonText = MarshalString(customButton.GetText())
                                      };
                    }
                }

                if (managed.RadioButtons.Count > 0)
                {
                    Align(ref pCurrent);
                    var pRadioButtons = (TASKDIALOG_BUTTON*) pCurrent;

                    unmanaged.pRadioButtons = pRadioButtons;
                    unmanaged.cRadioButtons = (uint) managed.RadioButtons.Count;

                    pCurrent += sizeof(TASKDIALOG_BUTTON) * managed.RadioButtons.Count;

                    Align(ref pCurrent, sizeof(char));

                    int i = 0;

                    foreach (TaskDialogRadioButton radioButton in managed.RadioButtons)
                    {
                        pRadioButtons[i++] = new TASKDIALOG_BUTTON
                                             {
                                                 nButtonID = radioButton.Id,
                                                 pszButtonText = MarshalString(radioButton.Text)
                                             };
                    }
                }

                // Task dialogs sure require some complicated marshalling, but we're all done.
                _unmanaged = unmanaged;

                // The native API doesn't modify the configuration we provide to it, so we just store the managed instance
                // so we can easily return it when ToManaged is called after the unmanaged function returns.
                _managed = managed;
                
                char* MarshalString(string? str)
                {
                    if (str is null)
                        return null;

                    fixed (char* pStr = str)
                    {
                        long bytesToCopy = SizeOfString(str);
                        Buffer.MemoryCopy(pStr, pCurrent, bytesToCopy, bytesToCopy);
                        
                        var pReturn = pCurrent;
                        pCurrent += bytesToCopy;

                        return (char*) pReturn;
                    }
                }
            }
            catch
            {
                Marshal.FreeHGlobal(_pTaskConfig);
                throw;
            }
        }

        /// <summary>
        /// Provides the unmanaged task dialog configuration currently loaded into the marshaller.
        /// </summary>
        /// <returns>The converted <see cref="TASKDIALOGCONFIG"/> value.</returns>
        public readonly TASKDIALOGCONFIG ToUnmanaged()
            => _unmanaged;

        /// <summary>
        /// Loads the provided unmanaged task dialog configuration into the marshaller.
        /// </summary>
        /// <param name="unmanaged">The unmanaged task dialog configuration.</param>
        public void FromUnmanaged(TASKDIALOGCONFIG unmanaged)
            => _unmanaged = unmanaged;

        /// <summary>
        /// Returns the managed counterpart to the previously created unmanaged task dialog configuration.
        /// </summary>
        /// <returns>The <see cref="TaskDialogConfiguration"/> instance originally provided for marshalling.</returns>
        /// <remarks>
        /// Although <see cref="TASKDIALOGCONFIG"/> is passed by reference, the native API doesn't modify anything in it,
        /// so there is no point in creating a new <see cref="TaskDialogConfiguration"/> instance from its unmanaged version.
        /// We just simply return what was originally provided to <see cref="FromManaged"/>.
        /// </remarks>
        public readonly TaskDialogConfiguration ToManaged()
            => _managed;

        /// <summary>
        /// Releases all resources in use by the marshaller.
        /// </summary>
        public readonly void Free()
        {   // Look at this, just a single deallocation required! Very nice.
            Marshal.FreeHGlobal(_pTaskConfig);

            if (_ownerHandle != null && _ownerHandleAddRefd)
                _ownerHandle.DangerousRelease();
        }

        private static byte* CalculateSizeToAllocate(TaskDialogConfiguration managed, List<TaskDialogButton> customButtons)
        {   // Start with the size of the config struct itself.
            var sizeToAllocate = (byte*) sizeof(TASKDIALOGCONFIG);

            // Next the task dialog strings, which we want aligned on a 2-byte boundary.
            Align(ref sizeToAllocate, sizeof(char));
            
            // Pointer remains aligned to 2-byte boundary following each string measurement, since it gets multiplied by sizeof(char).
            sizeToAllocate += SizeOfString(managed.Title);
            sizeToAllocate += SizeOfString(managed.InstructionText);
            sizeToAllocate += SizeOfString(managed.Text);
            sizeToAllocate += SizeOfString(managed.VerificationText);
            sizeToAllocate += SizeOfString(managed.ExpandedText);
            sizeToAllocate += SizeOfString(managed.ExpandedLabel);
            sizeToAllocate += SizeOfString(managed.CollapsedLabel);
            sizeToAllocate += SizeOfString(managed.FooterText);

            if (customButtons.Count > 0)
            {   // Next is the pointer to push button struct array, align the pointer to the word-sized boundary typically used for pointers.
                Align(ref sizeToAllocate);

                sizeToAllocate += sizeof(TASKDIALOG_BUTTON) * customButtons.Count;
                // The only members in our button structs are strings, so align to a 2-byte boundary again.
                Align(ref sizeToAllocate, sizeof(char));
                
                foreach (TaskDialogButton customButton in customButtons)
                {
                    sizeToAllocate += SizeOfString(customButton.GetText());
                }
            }

            if (managed.RadioButtons.Count > 0)
            {   // Do the same as above for our radio button struct array.
                Align(ref sizeToAllocate);

                sizeToAllocate += sizeof(TASKDIALOG_BUTTON) * managed.RadioButtons.Count;

                Align(ref sizeToAllocate, sizeof(char));

                foreach (TaskDialogRadioButton radioButton in managed.RadioButtons)
                {
                    sizeToAllocate += SizeOfString(radioButton.Text);
                }
            }

            return sizeToAllocate;
        }

        /// <summary>
        /// This callback function is directly invoked from native code, so we grab the pointer to our <see cref="TaskDialog"/>
        /// instance originally stored in the <see cref="TASKDIALOGCONFIG.lpCallbackData"/> member, which is provided here
        /// as the value of the <paramref name="lpRefData"/> parameter, and then invoke its managed callback.
        /// </summary>
        [UnmanagedCallersOnly]
        private static int NativeCallbackProc(IntPtr hWnd, 
                                              TaskDialogNotification msg, 
                                              IntPtr wParam, 
                                              IntPtr lParam,
                                              IntPtr lpRefData)
        {
            if (((GCHandle) lpRefData).Target is not TaskDialog taskDialog)
                throw new InvalidOperationException(Strings.TaskDialogCallbackHandleNotSet);

            return (int) taskDialog.TaskDialogCallbackProc(hWnd, msg, wParam, lParam);
        }
          
        private static int SizeOfString(string? str)
            // The additional character accounts for the unmanaged string's null terminator.
            => str is null ? 0 : (str.Length + 1) * sizeof(char);

        private static void Align(ref byte* p, int? alignment = null)
        {   // Standard alignment algorithm.
            IntPtr offset = (IntPtr) (alignment ?? IntPtr.Size) - 1;

            p = (byte*) (((IntPtr) p + offset) & ~offset);
        }

        /// <summary>
        /// Represents the information used to display a task dialog.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct TASKDIALOGCONFIG
        {
            /// <summary>
            /// Specifies the structure size, in bytes.
            /// </summary>
            public uint cbSize;
            /// <summary>
            /// Handle to the parent window, if there is one.
            /// </summary>
            public IntPtr hwndParent;
            /// <summary>
            /// Handle to the module that contains icon and string resources. We don't use this.
            /// </summary>
            public IntPtr hInstance;
            /// <summary>
            /// Specifies the behavior of the task dialog.
            /// </summary>
            public TaskDialogFlags dwFlags;
            /// <summary>
            /// Specifies the push buttons displayed in the task dialog.
            /// </summary>
            public TaskDialogButtonFlags dwCommonButtons;
            /// <summary>
            /// Pointer that references the string to be used for the task dialog title.
            /// </summary>
            public char* pszWindowTitle;
            /// <summary>
            /// The configuration for the task dialog's main icon.
            /// </summary>
            public IconUnion mainIcon;
            /// <summary>
            /// Pointer that references the string to be used for the main instruction.
            /// </summary>
            public char* pszMainInstruction;
            /// <summary>
            /// Pointer that references the string to be used for the dialog's primary content.
            /// </summary>
            public char* pszContent;
            /// <summary>
            /// THe number of entries for in the <see cref="pButtons"/> array that is used to create buttons or command links
            /// in the task dialog.
            /// </summary>
            public uint cButtons;
            /// <summary>
            /// Pointer to an array of <see cref="TASKDIALOG_BUTTON"/> structures containing the definition of the custom buttons that are
            /// to be displayed in the task dialog.
            /// </summary>
            public TASKDIALOG_BUTTON* pButtons;
            /// <summary>
            /// The button ID of the default button for the task dialog.
            /// </summary>
            public int nDefaultButton;
            /// <summary>
            /// The number of entries in the <see cref="pRadioButtons"/> array that is used to create the radio buttons in the task dialog.
            /// </summary>
            public uint cRadioButtons;
            /// <summary>
            /// Pointer to an array of <see cref="TASKDIALOG_BUTTON"/> structures containing the definition of the radio buttons that are to
            /// be displayed in the task dialog.
            /// </summary>
            public TASKDIALOG_BUTTON* pRadioButtons;
            /// <summary>
            /// The button ID of the radio button that is selected by default.
            /// </summary>
            public int nDefaultRadioButton;
            /// <summary>
            /// Pointer that references the string to be used to label the verification check box.
            /// </summary>
            public char* pszVerificationText;
            /// <summary>
            /// Pointer that references the string to be used for displaying additional information.
            /// </summary>
            public char* pszExpandedInformation;
            /// <summary>
            /// Pointer that references the string to be used to label the button for collapsing the expandable information.
            /// </summary>
            public char* pszExpandedControlText;
            /// <summary>
            /// Pointer that references the string to be used to label the button for expanding the expandable information.
            /// </summary>
            public char* pszCollapsedControlText;
            /// <summary>
            /// The configuration for the task dialog's footer icon.
            /// </summary>
            public IconUnion footerIcon;
            /// <summary>
            /// Pointer to the string to be used in the footer area of the task dialog.
            /// </summary>
            public char* pszFooter;
            /// <summary>
            /// Pointer to an application-defined callback function.
            /// </summary>
            public delegate* unmanaged<IntPtr, TaskDialogNotification, IntPtr, IntPtr, IntPtr, int> pfCallback;
            /// <summary>
            /// A pointer to application-defined reference data, which is passed to the callback.
            /// </summary>
            public IntPtr lpCallbackData;
            /// <summary>
            /// The width of the task dialog's client area, in dialog units.
            /// </summary>
            public uint cxWidth;

            /// <summary>
            /// Represents the configuration for a task dialog icon.
            /// </summary>
            [StructLayout(LayoutKind.Explicit, Pack = 1)]
            public struct IconUnion
            {
                /// <summary>
                /// A handle to an icon that is to be displayed in the task dialog. Used for custom icons.
                /// </summary>
                [FieldOffset(0)] public IntPtr hIcon;
                /// <summary>
                /// Pointer that references the icon to be displayed in the task dialog. Used for loading predefined, standard
                /// icons.
                /// </summary>
                [FieldOffset(0)] public char* pszIcon;
            }
        }

        /// <summary>
        /// Represents information used to display a button in a task dialog.
        /// </summary>
        /// <suppressions>
        /// ReSharper disable InconsistentNaming
        /// </suppressions>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct TASKDIALOG_BUTTON
        {
            /// <summary>
            /// The identifier for the button; this will be returned when the button is selected.
            /// </summary>
            public int nButtonID;

            /// <summary>
            /// Pointer that references the string to be used to label the button.
            /// </summary>
            public char* pszButtonText;
        }
    }
}
