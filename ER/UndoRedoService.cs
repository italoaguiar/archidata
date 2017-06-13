using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ER
{
    public class StateManager<T>: IUndoRedoNotifier
    {
        private Stack<T> UndoStack;
        private Stack<T> RedoStack;

        public StateManager()
        {
            UndoStack = new Stack<T>();
            RedoStack = new Stack<T>();
        }

        public bool CanUndo { get; set; }
        public bool CanRedo { get; set; }

        public T Undo()
        {
            CheckState();

            if (CanUndo)
            {
                OnStateChanged();

                var currentState = UndoStack.Pop();
                RedoStack.Push(currentState);
                return UndoStack.Peek();
            }
            else return UndoStack.Peek();
        }

        public T Redo()
        {
            CheckState();

            if (CanRedo)
            {
                OnStateChanged();

                var currentState = RedoStack.Pop();
                UndoStack.Push(currentState);
                return currentState;
            }
            else return UndoStack.Peek();
        }

        public void NotifyChange(object o)
        {
            NotifyChange((T)o);
        }

        public void NotifyChange(T o)
        {
            UndoStack.Push(o);

            Thread.Sleep(500);

            CheckState();

            OnStateChanged();         
        }

        private void CheckState()
        {
            CanUndo = UndoStack.Count > 1;
            CanRedo = RedoStack.Count > 0;
        }

        private void OnStateChanged()
        {
            StateChanged?.Invoke(this, null);
        }

        public event EventHandler StateChanged;

    }

    public interface IUndoRedoNotifier
    {
        void NotifyChange(object o);
    }

    public class StateService
    {
        public StateService(IUndoRedoNotifier r, Func<object> getObject)
        {
            Notify = () =>
            {
                r.NotifyChange(getObject());
            };
        }

        private static Action Notify;
        
        public static void NotifyChange()
        {
            if(enabled)
                Notify();
        }

        private static bool enabled = true;

        public static void Enable()
        {
            enabled = true;
        }
        public static void Disable()
        {
            enabled = false;
        }
    }

    
}
