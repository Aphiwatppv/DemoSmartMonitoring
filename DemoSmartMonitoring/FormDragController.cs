using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoSmartMonitoring
{
    /// <summary>
    /// Drop-in component to make a Form draggable via one or more handle Controls.
    /// - Set TargetForm (optional; otherwise inferred from handle.FindForm()).
    /// - Call Attach(handle) for each control that should drag the form.
    /// - Call Detach(handle) to remove.
    /// Works without Win32 P/Invoke; great for borderless forms.
    /// </summary>
    [DesignerCategory("Code")]
    public class FormDragController : Component
    {
        private readonly List<Control> _handles = new List<Control>();
        private bool _dragging;
        private Point _dragCursor;
        private Point _dragForm;

        public FormDragController() { }
        public FormDragController(IContainer container)
        {
            container?.Add(this);
        }

        /// <summary>
        /// Optional. If null, the form will be inferred from the first handle via handle.FindForm().
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        public Form TargetForm { get; set; }

        /// <summary>
        /// Attach a control as a drag handle.
        /// </summary>
        public void Attach(Control handle)
        {
            if (handle == null) return;
            if (_handles.Contains(handle)) return;

            handle.MouseDown += Handle_MouseDown;
            handle.MouseMove += Handle_MouseMove;
            handle.MouseUp += Handle_MouseUp;
            _handles.Add(handle);
        }

        /// <summary>
        /// Detach a control previously attached.
        /// </summary>
        public void Detach(Control handle)
        {
            if (handle == null) return;
            if (!_handles.Contains(handle)) return;

            handle.MouseDown -= Handle_MouseDown;
            handle.MouseMove -= Handle_MouseMove;
            handle.MouseUp -= Handle_MouseUp;
            _handles.Remove(handle);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Unhook everything to avoid leaks
                foreach (var h in _handles.ToArray())
                {
                    Detach(h);
                }
            }
            base.Dispose(disposing);
        }

        private void Handle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            var form = TargetForm ?? (sender as Control)?.FindForm();
            if (form == null) return;

            _dragging = true;
            _dragCursor = Cursor.Position;
            _dragForm = form.Location;
        }

        private void Handle_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_dragging) return;

            var form = TargetForm ?? (sender as Control)?.FindForm();
            if (form == null) return;

            Point diff = Point.Subtract(Cursor.Position, new Size(_dragCursor));
            form.Location = Point.Add(_dragForm, new Size(diff));
        }

        private void Handle_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }
    }
}
