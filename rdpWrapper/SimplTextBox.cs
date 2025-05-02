using System;
using System.Drawing;
using System.Windows.Forms;

namespace rdpWrapper {

  public class SimplTextBox : RichTextBox {

    private const short WM_PAINT = 0x00f;
    private const int MAXSIZE = 409600;
    private const int DROPSIZE = MAXSIZE / 4;

    private bool skipPainting;

    protected override void WndProc(ref Message m) {
      if (m.Msg == WM_PAINT && skipPainting)
        m.Result = IntPtr.Zero;
      else
        base.WndProc(ref m);
    }

    public void AppendLine(string text, Color color, bool newLine = true) {
      if (newLine)
        AppendText(Environment.NewLine);
      if (Text.Length > MAXSIZE) {
        skipPainting = true;
        var endmarker = Text.IndexOf('\n', DROPSIZE) + 1;
        if (endmarker < DROPSIZE)
          endmarker = DROPSIZE;
        Select(0, endmarker);//Select(0, GetFirstCharIndexFromLine(1000));
        var prevReadOnly = ReadOnly;
        ReadOnly = false;
        SelectedText = string.Empty;
        ReadOnly = prevReadOnly;
        skipPainting = false;
      }
      SelectionStart = Text.Length;
      SelectionLength = 0;
      SelectionColor = color;
      AppendText(text.Trim());
      //ScrollToCaret();
    }
  }
}