using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollShared
{
	class NativeMethods
	{
		public enum MouseEventSource
		{
			/// <summary>
			/// Events raised by the mouse
			/// </summary>
			Mouse,

			/// <summary>
			/// Events raised by a stylus
			/// </summary>
			Pen,

			/// <summary>
			/// Events raised by touching the screen
			/// </summary>
			Touch
		}

		[DllImport("user32.dll", SetLastError = false)]
		public static extern IntPtr GetMessageExtraInfo();

		/// <summary>
		/// Determines what input device triggered the mouse event.
		/// </summary>
		/// <returns>
		/// A result indicating whether the last mouse event was triggered
		/// by a touch, pen or the mouse.
		/// </returns>
		public static MouseEventSource GetMouseEventSource()
		{
			var extra = (uint)GetMessageExtraInfo();
			bool isTouchOrPen = (extra & 0xFFFFFF00) == 0xFF515700;

			if (!isTouchOrPen)
			{
				return MouseEventSource.Mouse;
			}

			bool isTouch = ((extra & 0x00000080) == 0x00000080);

			return isTouch ? MouseEventSource.Touch : MouseEventSource.Pen;
		}

		public static bool IsMouseEvent()
		{
			return GetMouseEventSource() == MouseEventSource.Mouse;
		}

		/// <summary>
		/// Gets high bits values of the pointer.
		/// </summary>
		public static int HIWORD(IntPtr ptr)
		{
			var val32 = ptr.ToInt32();
			return ((val32 >> 16) & 0xFFFF);
		}
	}
}
