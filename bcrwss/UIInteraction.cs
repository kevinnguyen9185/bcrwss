using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bcrwss
{
    internal class UIInteraction
    {
        ListBox _lstLog;
        ToolStripStatusLabel _toolStripStatusMain;
        ToolStripStatusLabel _toolStripStatusEvoSessionId;
        Form _frm;

        public UIInteraction(ListBox lstLog,
                             ToolStripStatusLabel toolStripStatusMain,
                             ToolStripStatusLabel toolStripStatusEvoSessionId,
                             Form frm)
        {
            _lstLog = lstLog;
            _toolStripStatusMain = toolStripStatusMain;
            _toolStripStatusEvoSessionId = toolStripStatusEvoSessionId;
            _frm = frm;
        }

        public void Log(string msg)
        {
            _frm.Invoke((MethodInvoker)(() => {
                _lstLog.Items.Add(msg);
                if (_lstLog.Items.Count > 1000)
                {
                    _lstLog.Items.RemoveAt(1000);
                }
            }));
        }

        public void UpdateToolTipMain(string msg)
        {
            _frm.Invoke((MethodInvoker)(() => {
                _toolStripStatusMain.Text = msg;
            }));
        }

        public void UpdateToolTipSessionId(string msg)
        {
            _frm.Invoke((MethodInvoker)(() => {
                _toolStripStatusEvoSessionId.Text = msg;
            }));
        }
    }
}

 