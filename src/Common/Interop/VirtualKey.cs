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

namespace BadEcho.Interop;

/// <summary>
/// Specifies a virtual key, which may be anything from an actual keyboard key to a button on the mouse.
/// </summary>
public enum VirtualKey
{
    /// <summary>
    /// A key that does not exist. The complete absence of a key. A key with no name, no purpose, and no hope.
    /// </summary>
    None = 0,
    /// <summary>
    /// The left mouse button.
    /// </summary>
    LeftButton,
    /// <summary>
    /// The right mouse button.
    /// </summary>
    RightButton,
    /// <summary>
    /// Control-break processing (Ctrl + Pause).
    /// </summary>
    Cancel,
    /// <summary>
    /// The middle mouse button.
    /// </summary>
    MiddleButton,
    /// <summary>
    /// The X1 mouse button.
    /// </summary>
    X1Button,
    /// <summary>
    /// The X2 mouse button.
    /// </summary>
    X2Button,
    /// <summary>
    /// The Backspace key.
    /// </summary>
    Backspace = 0x8,
    /// <summary>
    /// The Tab key.
    /// </summary>
    Tab,
    /// <summary>
    /// The Clear key.
    /// </summary>
    Clear = 0xC,
    /// <summary>
    /// The Enter key.
    /// </summary>
    Enter,
    /// <summary>
    /// The Shift key.
    /// </summary>
    Shift = 0x10,
    /// <summary>
    /// The Ctrl key.
    /// </summary>
    Control,
    /// <summary>
    /// The Alt key.
    /// </summary>
    Alt,
    /// <summary>
    /// The Pause key.
    /// </summary>
    Pause,
    /// <summary>
    /// The Caps Lock key.
    /// </summary>
    CapsLock,
    /// <summary>
    /// The Escape key.
    /// </summary>
    Escape = 0x1B,
    /// <summary>
    /// The Spacebar key.
    /// </summary>
    Space = 0x20,
    /// <summary>
    /// The Page Up key.
    /// </summary>
    PageUp,
    /// <summary>
    /// The Page Down key.
    /// </summary>
    PageDown,
    /// <summary>
    /// The End key.
    /// </summary>
    End,
    /// <summary>
    /// The Home key.
    /// </summary>
    Home,
    /// <summary>
    /// The Left arrow key.
    /// </summary>
    Left,
    /// <summary>
    /// The Up arrow key.
    /// </summary>
    Up,
    /// <summary>
    /// The Right arrow key.
    /// </summary>
    Right,
    /// <summary>
    /// The Down arrow key.
    /// </summary>
    Down,
    /// <summary>
    /// The Select key.
    /// </summary>
    Select,
    /// <summary>
    /// The Print key. Not the Print <c>Screen</c> key, mind you.
    /// </summary>
    Print,
    /// <summary>
    /// The Execute key. 
    /// </summary>
    Execute,
    /// <summary>
    /// The Print Screen key.
    /// </summary>
    PrintScreen,
    /// <summary>
    /// The Insert key.
    /// </summary>
    Insert,
    /// <summary>
    /// The Delete key.
    /// </summary>
    Delete,
    /// <summary>
    /// The Help key. Haven't seen this one in a while!
    /// </summary>
    Help,
    /// <summary>
    /// The 0 key.
    /// </summary>
    Zero,
    /// <summary>
    /// The 1 key.
    /// </summary>
    One,
    /// <summary>
    /// The 2 key.
    /// </summary>
    Two,
    /// <summary>
    /// The 3 key.
    /// </summary>
    Three,
    /// <summary>
    /// The 4 key.
    /// </summary>
    Four,
    /// <summary>
    /// The 5 key.
    /// </summary>
    Five,
    /// <summary>
    /// The 6 key.
    /// </summary>
    Six,
    /// <summary>
    /// The 7 key.
    /// </summary>
    Seven,
    /// <summary>
    /// The 8 key.
    /// </summary>
    Eight,
    /// <summary>
    /// The 9 key.
    /// </summary>
    Nine,
    /// <summary>
    /// The A key.
    /// </summary>
    A =  0x41,
    /// <summary>
    /// The B key.
    /// </summary>
    B,
    /// <summary>
    /// The C key.
    /// </summary>
    C,
    /// <summary>
    /// The D key.
    /// </summary>
    D,
    /// <summary>
    /// The E key.
    /// </summary>
    E,
    /// <summary>
    /// The F key.
    /// </summary>
    F,
    /// <summary>
    /// The G key.
    /// </summary>
    G,
    /// <summary>
    /// The H key.
    /// </summary>
    H,
    /// <summary>
    /// The I key.
    /// </summary>
    I,
    /// <summary>
    /// The J key.
    /// </summary>
    J,
    /// <summary>
    /// The K key.
    /// </summary>
    K,
    /// <summary>
    /// The L key.
    /// </summary>
    L,
    /// <summary>
    /// The M key.
    /// </summary>
    M,
    /// <summary>
    /// The N key.
    /// </summary>
    N,
    /// <summary>
    /// The O key.
    /// </summary>
    O,
    /// <summary>
    /// The P key.
    /// </summary>
    P,
    /// <summary>
    /// The Q key.
    /// </summary>
    Q,
    /// <summary>
    /// The R key.
    /// </summary>
    R,
    /// <summary>
    /// The S key.
    /// </summary>
    S,
    /// <summary>
    /// The T key.
    /// </summary>
    T,
    /// <summary>
    /// The U key.
    /// </summary>
    U,
    /// <summary>
    /// The V key.
    /// </summary>
    V,
    /// <summary>
    /// The W key.
    /// </summary>
    W,
    /// <summary>
    /// The X key.
    /// </summary>
    X,
    /// <summary>
    /// The Y key.
    /// </summary>
    Y,
    /// <summary>
    /// The Z key.
    /// </summary>
    Z,
    /// <summary>
    /// The left Windows key.
    /// </summary>
    LeftWindows,
    /// <summary>
    /// The right Windows key.
    /// </summary>
    RightWindows,
    /// <summary>
    /// The Application key.
    /// </summary>
    Apps,
    /// <summary>
    /// The computer sleep key.
    /// </summary>
    Sleep = 0x5f,
    /// <summary>
    /// The 0 key on the numeric keypad.
    /// </summary>
    NumPad0,
    /// <summary>
    /// The 1 key on the numeric keypad.
    /// </summary>
    NumPad1,
    /// <summary>
    /// The 2 key on the numeric keypad.
    /// </summary>
    NumPad2,
    /// <summary>
    /// The 3 key on the numeric keypad.
    /// </summary>
    NumPad3,
    /// <summary>
    /// The 4 key on the numeric keypad.
    /// </summary>
    NumPad4,
    /// <summary>
    /// The 5 key on the numeric keypad.
    /// </summary>
    NumPad5,
    /// <summary>
    /// The 6 key on the numeric keypad.
    /// </summary>
    NumPad6,
    /// <summary>
    /// The 7 key on the numeric keypad.
    /// </summary>
    NumPad7,
    /// <summary>
    /// The 8 key on the numeric keypad.
    /// </summary>
    NumPad8,
    /// <summary>
    /// The 9 key on the numeric keypad.
    /// </summary>
    NumPad9,
    /// <summary>
    /// The multiply key.
    /// </summary>
    Multiply,
    /// <summary>
    /// The add key.
    /// </summary>
    Add,
    /// <summary>
    /// The separator key.
    /// </summary>
    Separator,
    /// <summary>
    /// The subtract key.
    /// </summary>
    Subtract,
    /// <summary>
    /// The decimal key.
    /// </summary>
    Decimal,
    /// <summary>
    /// The divide key.
    /// </summary>
    Divide,
    /// <summary>
    /// The F1 key.
    /// </summary>
    F1,
    /// <summary>
    /// The F2 key.
    /// </summary>
    F2,
    /// <summary>
    /// The F3 key.
    /// </summary>
    F3,
    /// <summary>
    /// The F4 key.
    /// </summary>
    F4,
    /// <summary>
    /// The F5 key.
    /// </summary>
    F5,
    /// <summary>
    /// The F6 key.
    /// </summary>
    F6,
    /// <summary>
    /// The F7 key.
    /// </summary>
    F7,
    /// <summary>
    /// The F8 key.
    /// </summary>
    F8,
    /// <summary>
    /// The F9 key.
    /// </summary>
    F9,
    /// <summary>
    /// The F10 key.
    /// </summary>
    F10,
    /// <summary>
    /// The F11 key.
    /// </summary>
    F11,
    /// <summary>
    /// The F12 key.
    /// </summary>
    F12,
    /// <summary>
    /// The F13 key. Yes, they exist.
    /// </summary>
    F13,
    /// <summary>
    /// The F14 key.
    /// </summary>
    F14,
    /// <summary>
    /// The F15 key.
    /// </summary>
    F15,
    /// <summary>
    /// The F16 key.
    /// </summary>
    F16,
    /// <summary>
    /// The F17 key.
    /// </summary>
    F17,
    /// <summary>
    /// The F18 key.
    /// </summary>
    F18,
    /// <summary>
    /// The F19 key.
    /// </summary>
    F19,
    /// <summary>
    /// The F20 key.
    /// </summary>
    F20,
    /// <summary>
    /// The F21 key.
    /// </summary>
    F21,
    /// <summary>
    /// The F22 key.
    /// </summary>
    F22,
    /// <summary>
    /// The F23 key.
    /// </summary>
    F23,
    /// <summary>
    /// The F24 key.
    /// </summary>
    F24,
    /// <summary>
    /// The Num Lock key.
    /// </summary>
    NumLock = 0x90,
    /// <summary>
    /// The Scroll Lock key.
    /// </summary>
    ScrollLock,
    /// <summary>
    /// The left Shift key.
    /// </summary>
    LeftShift = 0xA0,
    /// <summary>
    /// The right Shift key.
    /// </summary>
    RightShift,
    /// <summary>
    /// The left Ctrl key.
    /// </summary>
    LeftControl,
    /// <summary>
    /// The right Ctrl key.
    /// </summary>
    RightControl,
    /// <summary>
    /// The left Alt key.
    /// </summary>
    LeftAlt,
    /// <summary>
    /// The right Alt key.
    /// </summary>
    RightAlt,
    /// <summary>
    /// The semicolon key.
    /// </summary>
    Semicolon= 0xBA,
    /// <summary>
    /// The equals key.
    /// </summary>
    Equals,
    /// <summary>
    /// The comma key.
    /// </summary>
    Comma,
    /// <summary>
    /// The minus key.
    /// </summary>
    Minus,
    /// <summary>
    /// The period key.
    /// </summary>
    Period,
    /// <summary>
    /// The forward slash key.
    /// </summary>
    ForwardSlash,
    /// <summary>
    /// The tilde key.
    /// </summary>
    Tilde,
    /// <summary>
    /// The open bracket key.
    /// </summary>
    LeftBracket = 0xDB,
    /// <summary>
    /// The backslash key.
    /// </summary>
    Backslash,
    /// <summary>
    /// The closing bracket key.
    /// </summary>
    RightBracket,
    /// <summary>
    /// The apostrophe key.
    /// </summary>
    Apostrophe
}