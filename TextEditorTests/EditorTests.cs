using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor.Tests
{
    [TestClass()]
    public class EditorTests
    {
        Editor e = new Editor();

        [TestInitialize]
        public void BeforeTests()
        {

        }


        [TestMethod()]
        public void UndoTest()
        {
            if(e.CanUndo) e.Undo();
        }

        [TestMethod()]
        public void RedoTest()
        {
            if (e.CanRedo) e.Redo();
        }

        [TestMethod()]
        public void CopyTest()
        {
            if (e.CanCopy) e.Copy();
        }

        [TestMethod()]
        public void CutTest()
        {
            if (e.CanCut) e.Cut();
        }

        [TestMethod()]
        public void PasteTest()
        {
            if (e.CanPaste) e.Paste();

            int x = 0;

            int p = 10 / x;
        }


        [TestMethod()]
        public void OpenTest()
        {
            var validUrl = @"C:\Users\italo\Documents\MAP.txt";
            var invalidUrl = @"C:\invalid.txt";
            e.Open(validUrl);
            e.Open(invalidUrl);
        }
    }
}