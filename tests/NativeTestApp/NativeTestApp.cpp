#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
#include <windows.h>

namespace
{
	LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
	{
		switch (message) {
		case WM_PAINT: {

			PAINTSTRUCT ps;
			BeginPaint(hWnd, &ps);
			EndPaint(hWnd, &ps);
			break;
		}

		case WM_DESTROY: {
			PostQuitMessage(0);
			break;
		}

		default: {
			return DefWindowProc(hWnd, message, wParam, lParam);
		}
		}

		return 0;
	}
}

int APIENTRY wWinMain(_In_ HINSTANCE hInstance,	_In_opt_ HINSTANCE,	_In_ LPWSTR, _In_ int)
{
	WNDCLASSEXW windowClass{
		sizeof(WNDCLASSEX),
		0,
		WndProc,
		0,
		0,
		hInstance,
		nullptr,
		LoadCursor(nullptr, IDC_ARROW),
		(HBRUSH)(COLOR_WINDOW + 1),
		nullptr,
		L"NativeTestApp Window Class",
		nullptr
	};

	RegisterClassExW(&windowClass);

	HWND hWnd = CreateWindowW(
		L"NativeTestApp Window Class",
		L"NativeTestApp",
		WS_OVERLAPPEDWINDOW,
		CW_USEDEFAULT,
		0,
		CW_USEDEFAULT,
		0,
		nullptr,
		nullptr,
		hInstance,
		nullptr);

	ShowWindow(hWnd, SW_SHOWMINIMIZED);
	UpdateWindow(hWnd);

	MSG msg;

	while (GetMessage(&msg, nullptr, 0, 0))
	{
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}

	return 0;
}