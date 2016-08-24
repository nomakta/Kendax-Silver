using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Sulakore.Protocol.Components
{
    public class HMConstructer : ListView
    {
        private List<object> HMChunks;
        private ColumnHeader TypeCol, ValueCol, EncodedCol;

        private HDestinations _Destination = HDestinations.Server;
        public HDestinations Destination
        {
            get { return _Destination; }
            set
            {
                if (value != HDestinations.Unknown && value != _Destination)
                {
                    _Destination = value;
                    if (_Protocol == HProtocols.Ancient && HMChunks.Count > 0)
                        ReconstructList(true);
                }
            }
        }

        private HProtocols _Protocol = HProtocols.Modern;
        public HProtocols Protocol
        {
            get { return _Protocol; }
            set
            {
                if (value != HProtocols.Unknown && value != _Protocol)
                {
                    _Protocol = value;
                    if (HMChunks.Count > 0)
                        ReconstructList(false);
                }
            }
        }

        public bool LockColumns { get; set; }

        public HMConstructer()
            : base()
        {
            HMChunks = new List<object>();
            TypeCol = new ColumnHeader() { Name = "TypeCol", Text = "Type" };
            ValueCol = new ColumnHeader() { Name = "ValueCol", Text = "Value", Width = 194 };
            EncodedCol = new ColumnHeader() { Name = "EncodedCol", Text = "Encoded", Width = 105 };
            Columns.AddRange(new ColumnHeader[3] { TypeCol, ValueCol, EncodedCol });
            FullRowSelect = true;
            GridLines = true;
            HeaderStyle = ColumnHeaderStyle.Nonclickable;
            MultiSelect = false;
            ShowItemToolTips = true;
            Size = new Size(386, 166);
            UseCompatibleStateImageBehavior = false;
            View = View.Details;
            LockColumns = true;
        }

        public void AppendChunk(int Value)
        {
            HMChunks.Add(Value);
            string Encoded = HMessage.ToString(HMessage.ConstructBody(Destination, Protocol, Value));
            AddItemChunk("Integer", Value, Encoded);
        }
        public void AppendChunk(bool Value)
        {
            HMChunks.Add(Value);
            string Encoded = HMessage.ToString(HMessage.ConstructBody(Destination, Protocol, Value));
            AddItemChunk("Boolean", Value, Encoded);
        }
        public void AppendChunk(string Value)
        {
            HMChunks.Add(Value);

            string EncodedLength = string.Empty;
            string Encoded = HMessage.ToString(HMessage.ConstructBody(Destination, Protocol, Value));

            if ((Destination == HDestinations.Server && Protocol == HProtocols.Ancient) || Protocol == HProtocols.Modern)
                EncodedLength = (Protocol == HProtocols.Modern ? HMessage.ToString(BigEndian.CypherShort((ushort)Value.Length)) : HMessage.ToString(Ancient.CypherShort((ushort)Value.Length))) + " | ";

            AddItemChunk("String", Value, Encoded, string.Format("Length: {0}{1}\n", EncodedLength, Value.Length));
        }
        private void AddItemChunk(string Type, object Value, string Encoded, string ExtraStringInfo = null)
        {
            ListViewItem Item = new ListViewItem(new string[3] { Type, Value.ToString(), Encoded });
            Item.ToolTipText = string.Format("Type: {0}\nValue: {1}\n{2}Encoded: {3}", Type, Value, ExtraStringInfo, Encoded);

            Focus();
            Items.Add(Item);
            Item.Selected = true;
            EnsureVisible(Items.Count - 1);
        }

        public void RemoveSelected()
        {
            int Index = SelectedIndices[0];
            HMChunks.RemoveAt(Index);
            Items.RemoveAt(Index);

            if (Items.Count > 0)
                Items[Index - (Index > Items.Count - 1 ? 1 : 0)].Selected = true;
        }
        public void MoveSelectedUp()
        {
            int Index = SelectedIndices[0];
            if (Index == 0) return;

            object ToMoveOBJ = HMChunks[Index];
            HMChunks.RemoveAt(Index);
            HMChunks.Insert(Index - 1, ToMoveOBJ);

            //Cache SubItems
            string[] ToPushUpItems = new string[4];
            for (int i = 0; i < Items[Index].SubItems.Count; i++)
                ToPushUpItems[i] = Items[Index].SubItems[i].Text;
            ToPushUpItems[3] = Items[Index].ToolTipText;

            string[] ToPushDownItems = new string[4];
            for (int i = 0; i < Items[Index - 1].SubItems.Count; i++)
                ToPushDownItems[i] = Items[Index - 1].SubItems[i].Text;
            ToPushDownItems[3] = Items[Index - 1].ToolTipText;

            //Switch
            for (int i = 0; i < 3; i++)
                Items[Index].SubItems[i].Text = ToPushDownItems[i];
            Items[Index].ToolTipText = ToPushDownItems[3];

            for (int i = 0; i < 3; i++)
                Items[Index - 1].SubItems[i].Text = ToPushUpItems[i];
            Items[Index - 1].ToolTipText = ToPushUpItems[3];

            //Focus / Highlight / Scroll
            Focus();
            Items[Index - 1].Selected = true;
            EnsureVisible(Index - 1);
        }
        public void MoveSelectedDown()
        {
            int Index = SelectedIndices[0];
            if (Index == Items.Count - 1) return;

            object ToMoveOBJ = HMChunks[Index];
            HMChunks.RemoveAt(Index);
            HMChunks.Insert(Index + 1, ToMoveOBJ);

            //Cache SubItems
            string[] ToPushDownItems = new string[4];
            for (int i = 0; i < Items[Index].SubItems.Count; i++)
                ToPushDownItems[i] = Items[Index].SubItems[i].Text;
            ToPushDownItems[3] = Items[Index].ToolTipText;

            string[] ToPushUpItems = new string[4];
            for (int i = 0; i < Items[Index + 1].SubItems.Count; i++)
                ToPushUpItems[i] = Items[Index + 1].SubItems[i].Text;
            ToPushUpItems[3] = Items[Index + 1].ToolTipText;

            //Switch
            for (int i = 0; i < 3; i++)
                Items[Index].SubItems[i].Text = ToPushUpItems[i];
            Items[Index].ToolTipText = ToPushUpItems[3];

            for (int i = 0; i < 3; i++)
                Items[Index + 1].SubItems[i].Text = ToPushDownItems[i];
            Items[Index + 1].ToolTipText = ToPushDownItems[3];

            //Focus / Highlight / Scroll
            Focus();
            Items[Index + 1].Selected = true;
            EnsureVisible(Index + 1);
        }
        public void ReplaceSelected(object Value)
        {
            int Index = SelectedIndices[0];
            if (Value.Equals(HMChunks[Index])) return;

            HMChunks[Index] = Value;
            ListViewItem CurItem = Items[Index];
            string Type = Value is string ? "String" : Value is int ? "Integer" : "Boolean";
            string Encoded = HMessage.ToString(HMessage.ConstructBody(Destination, Protocol, Value));
            Items[Index].SubItems[0].Text = Type;
            Items[Index].SubItems[1].Text = Value.ToString();
            Items[Index].SubItems[2].Text = Encoded;
            string EncodedLength = string.Empty;
            if (Value is string)
            {
                if ((Destination == HDestinations.Server && Protocol == HProtocols.Ancient) || Protocol == HProtocols.Modern)
                    EncodedLength = (Protocol == HProtocols.Modern ? HMessage.ToString(BigEndian.CypherShort((ushort)Value.ToString().Length)) : HMessage.ToString(Ancient.CypherShort((ushort)Value.ToString().Length))) + " | ";
            }
            Items[Index].ToolTipText = string.Format("Type: {0}\nValue: {1}\n{2}Encoded: {3}", Type, Value, string.Format("Length: {0}{1}\n", EncodedLength, Value.ToString().Length), Encoded);
        }

        public void ClearChunks()
        {
            HMChunks.Clear();
            Items.Clear();
        }
        public HMessage Construct(ushort Header)
        {
            return new HMessage(Header, Destination, Protocol, HMChunks.ToArray());
        }
        private void ReconstructList(bool StringsOnly)
        {
            BeginUpdate();
            for (int i = 0; i < HMChunks.Count; i++)
            {
                object Chunk = HMChunks[i];
                if (!(Chunk is string) && StringsOnly) continue;
                string Encoded = HMessage.ToString(HMessage.ConstructBody(Destination, Protocol, Chunk));
                Items[i].SubItems[2].Text = Encoded;
                if (Chunk is string)
                {
                    string Value = (string)Chunk;
                    string EncodedLength = string.Empty;
                    if ((Destination == HDestinations.Server && Protocol == HProtocols.Ancient) || Protocol == HProtocols.Modern)
                        EncodedLength = (Protocol == HProtocols.Modern ? HMessage.ToString(BigEndian.CypherShort((ushort)Value.Length)) : HMessage.ToString(Ancient.CypherShort((ushort)Value.Length))) + " | ";

                    Items[i].ToolTipText = string.Format("Type: String\nValue: {0}\n{1}Encoded: {2}", Value, string.Format("Length: {0}{1}\n", EncodedLength, Value.Length), Encoded);
                }
                else Items[i].ToolTipText = Items[i].ToolTipText.Replace(Items[i].ToolTipText.GetChild("Encoded: ", '\n'), Encoded);
            }
            EndUpdate();
        }

        protected override void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
        {
            if (LockColumns)
            {
                e.Cancel = true;
                e.NewWidth = Columns[e.ColumnIndex].Width;
            }
            base.OnColumnWidthChanging(e);
        }
    }
}