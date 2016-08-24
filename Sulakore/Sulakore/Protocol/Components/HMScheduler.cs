using System;
using System.Timers;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;

namespace Sulakore.Protocol.Components
{
    public class HMScheduler : ListView
    {
        private ColumnHeader PacketCol, DirectionCol, BurstCol, IntervalCol, StatusCol;
        public delegate void HMScheduleCallback(HMSchedule Schedule);

        private List<HMSchedule> Schedules = new List<HMSchedule>();

        public bool LockColumns { get; set; }

        public HMScheduler()
            : base()
        {
            PacketCol = new ColumnHeader() { Name = "PacketCol", Text = "Packet", Width = 138 };
            DirectionCol = new ColumnHeader() { Name = "DirectionCol", Text = "Direction", Width = 63 };
            BurstCol = new ColumnHeader() { Name = "BurstCol", Text = "Burst", Width = 44 };
            IntervalCol = new ColumnHeader() { Name = "IntervalCol", Text = "Interval", Width = 58 };
            StatusCol = new ColumnHeader() { Name = "StatusCol", Text = "Status", Width = 62 };
            Columns.AddRange(new ColumnHeader[5] { PacketCol, DirectionCol, BurstCol, IntervalCol, StatusCol });
            FullRowSelect = true;
            GridLines = true;
            HeaderStyle = ColumnHeaderStyle.Nonclickable;
            MultiSelect = false;
            ShowItemToolTips = true;
            Size = new Size(386, 141);
            UseCompatibleStateImageBehavior = false;
            View = View.Details;
            LockColumns = true;
        }

        public void AddSchedule(HMessage Packet, int Interval, int Burst, bool AutoStart, string Description, HMScheduleCallback Callback)
        {
            ListViewItem Item = new ListViewItem(new string[5] { Packet.ToString(), Packet.Destination.ToString(), Burst.ToString(), Interval.ToString(), "Running" });
            Item.ToolTipText = Description;

            Focus();
            Items.Add(Item);
            Item.Selected = true;
            EnsureVisible(Items.Count - 1);

            HMSchedule Schedule = new HMSchedule(Packet, Interval, Burst, Callback);
            Schedules.Add(Schedule);

            if (AutoStart) Schedule.Start();
        }

        public void StopAll()
        {
            foreach (HMSchedule Schedule in Schedules)
                Schedule.Stop();
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

        public class HMSchedule
        {
            private readonly HMScheduleCallback Callback;
            private readonly System.Timers.Timer Ticker;

            public int Burst { get; set; }
            public int Interval { get; set; }
            public HMessage Packet { get; set; }

            public HMSchedule(HMessage Packet, int Interval, int Burst, HMScheduleCallback Callback)
            {
                if (Burst < 1) throw new Exception("The burst value must be higher than one.");

                this.Packet = Packet;
                this.Interval = Interval;
                this.Burst = Burst;
                this.Callback = Callback;

                Ticker = new System.Timers.Timer(Interval);
                Ticker.Elapsed += Ticker_Elapsed;
            }
            private void Ticker_Elapsed(object sender, ElapsedEventArgs e)
            {
                Ticker.Stop();
                for (int i = 0; i < Burst; i++)
                {
                    Callback(this);
                }

                Ticker.Start();
            }
            public void Start()
            {
                if (!Ticker.Enabled)
                    Ticker.Start();
            }
            public void Stop()
            {
                if (Ticker.Enabled)
                    Ticker.Stop();
            }
        }
    }
}