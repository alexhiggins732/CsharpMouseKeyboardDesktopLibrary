using MouseKeyboardEvents;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GlobalMacroRecorder
{
    public class MouseKeyNetworkInitializer
    {
        public Rectangle Bounds;
        public int Height { get; private set; }
        public int Width { get; private set; }

        public double Scale { get; private set; }
        public MouseKeyNetwork Network;
        public MouseKeyNetworkInitializer(MouseKeyNetwork network)
        {
            Network = network;
            Scale = ScreenRecorder.ScalingUtil.GetScalingFactor();
            Height = (int)(Screen.PrimaryScreen.Bounds.Height * Scale);
            Width = (int)(Screen.PrimaryScreen.Bounds.Width * Scale);
            Bounds = new Rectangle(0, 0, Width, Height);
        }
        public void Update(MouseKeyEvent eventData)
        {

        }

        const int locationMask = 0x3FFF; // (1 << 14) - 1; 16383;
        const int yOffset = 14;
        const int buttonDataOffset = 28;

        public const int DefaultMaxTimeSinceLastEvent = 1000 * 30;
        public static int MaxTimeSinceLastEvent = DefaultMaxTimeSinceLastEvent;
        public void Update(ulong data, bool validate = true)
        {
            if (validate)
            {

                var errors = new List<string>();
                var word1 = (int)(uint)data;


                int macroEventTypeData = word1 & 7;
                MouseKeyEventType macroEventType = MouseKeyEventType.MouseMove;
                if (macroEventTypeData < 6)
                {
                    macroEventType = (MouseKeyEventType)(macroEventTypeData);
                }
                else
                {
                    errors.Add($"Invalid value for {nameof(MouseKeyEventType)}: {macroEventType}");
                }

                var timeSinceLastEvent = (word1 >> 3);
                if (timeSinceLastEvent >= MaxTimeSinceLastEvent)
                {
                    errors.Add($"{nameof(MouseKeyEvent.TimeSinceLastEvent)} ({timeSinceLastEvent}) is greater than {nameof(MaxTimeSinceLastEvent)} ({MaxTimeSinceLastEvent})");
                }



                var word2 = (int)(uint)(data >> 32);
                bool isKeyEvent = macroEventType == MouseKeyEventType.KeyDown | macroEventType == MouseKeyEventType.KeyUp;

                if (isKeyEvent)
                {

                    if (word2 == (int)Keys.KeyCode || word2 == (int)Keys.Modifiers)
                    {
                        errors.Add($"Invalid {nameof(KeyEventArgs)}.{nameof(KeyEventArgs.KeyData)} ({word2})");
                    }
                    else
                    {
                        int keyCodeData = word2 & (int)Keys.KeyCode;
                        int keyModifierData = word2 & (int)Keys.Modifiers;
                        if (!Enum.IsDefined(typeof(Keys), keyCodeData))
                        {
                            errors.Add($"Invalid {nameof(Keys)}.{nameof(Keys.KeyCode)} (keyCodeData)");
                        }
                        if (!Enum.IsDefined(typeof(Keys), keyModifierData))
                        {
                            errors.Add($"Invalid {nameof(KeyEventArgs)}.{nameof(KeyEventArgs.Modifiers)} {keyModifierData}");
                        }
                    }
                }
                else
                {
                    var mouseData = word2;


                    //int clicks = 1; //always 0
                    int x = mouseData & locationMask;// 14 bits: 0-13
                    int y = (mouseData >> yOffset) & locationMask;// 14 bits: 14-27;

                    if (x < 0 || x > Width)
                    {
                        errors.Add($"Mouse.X {x} must be between 0 and Screen.Width ({Width})");
                    }

                    if (y < 0 || y > Height)
                    {
                        errors.Add($"Mouse.Y {y} must be between 0 and Screen.Width ({Height})");
                    }

                    MouseButtons buttons = MouseButtons.None;
                    int buttonData = mouseData >> buttonDataOffset; // 3: bits 28-30;
                    int delta = 0;
                    switch (buttonData)
                    {
                        case 1: buttons = MouseButtons.Left; break;
                        case 2: buttons = MouseButtons.Right; break;
                        case 3: buttons = MouseButtons.Middle; break;
                        case 4: buttons = MouseButtons.XButton1; break;
                        case 5: buttons = MouseButtons.XButton2; break;
                        case 6: delta = 120; break;
                        case 7: delta = -120; break;
                    }

                    if (delta != 0)
                    {
                        if (macroEventType != MouseKeyEventType.MouseWheel)
                        {
                            errors.Add($"{nameof(MouseEventArgs.Delta)} {delta} can only be specified for {nameof(MouseKeyEventType)}.{nameof(MouseKeyEventType.MouseWheel)}.");
                        }
                    }
                    if (macroEventType == MouseKeyEventType.MouseWheel && delta == 0)
                    {
                        errors.Add($"{nameof(MouseEventArgs.Delta)} {delta} must be specified for {nameof(MouseKeyEventType)}.{nameof(MouseKeyEventType.MouseWheel)}.");
                    }
                    if (macroEventType == MouseKeyEventType.MouseWheel && buttons != MouseButtons.None)
                    {
                        errors.Add($"{nameof(MouseButtons)}.{nameof(MouseButtons.None)} must be specified for {nameof(MouseKeyEventType)}.{nameof(MouseKeyEventType.MouseWheel)}.");
                    }

                    if (macroEventType != MouseKeyEventType.MouseWheel && buttons == MouseButtons.None)
                    {
                        errors.Add($"{nameof(MouseButtons)}.{nameof(MouseButtons.None)} cannot be specified for {nameof(MouseKeyEventType)}.{nameof(MouseKeyEventType.MouseWheel)}.");
                    }
                }

                if (errors.Count > 0)
                {
                    var errorMessages = Enumerable.Range(0, errors.Count).Select(i => $"{i + 1}: {errors[i]}");
                    var inner = new Exception("\r\n\t" + string.Join("\r\n\t", (string[])errorMessages));
                    throw new Exception($"{errors.Count} validation errors occurred", inner);
                }
            }
            var neurons = Enumerable.Range(0, 64).Select(i => ((data & (1u << i)) > 0) ? 1.0 : 0.0).ToArray();
            Array.Copy(neurons, Network.Neurons, neurons.Length);
        }


    }

    public class MouseNetworkSingleton
    {
        public double Data { get; set; }
    }

    public class MouseNetworkOneHot
    {

    }

    public class MouseKeyNetwork
    {
        public double[] Neurons = new double[64];
        public double Bit_00 { get => Neurons[0]; set => Neurons[0] = value; }
        public double Bit_01 { get => Neurons[1]; set => Neurons[1] = value; }
        public double Bit_02 { get => Neurons[2]; set => Neurons[2] = value; }
        public double Bit_03 { get => Neurons[3]; set => Neurons[3] = value; }
        public double Bit_04 { get => Neurons[4]; set => Neurons[4] = value; }
        public double Bit_05 { get => Neurons[5]; set => Neurons[5] = value; }
        public double Bit_06 { get => Neurons[6]; set => Neurons[6] = value; }
        public double Bit_07 { get => Neurons[7]; set => Neurons[7] = value; }
        public double Bit_08 { get => Neurons[8]; set => Neurons[8] = value; }
        public double Bit_09 { get => Neurons[9]; set => Neurons[9] = value; }
        public double Bit_10 { get => Neurons[10]; set => Neurons[10] = value; }
        public double Bit_11 { get => Neurons[11]; set => Neurons[11] = value; }
        public double Bit_12 { get => Neurons[12]; set => Neurons[12] = value; }
        public double Bit_13 { get => Neurons[13]; set => Neurons[13] = value; }
        public double Bit_14 { get => Neurons[14]; set => Neurons[14] = value; }
        public double Bit_15 { get => Neurons[15]; set => Neurons[15] = value; }
        public double Bit_16 { get => Neurons[16]; set => Neurons[16] = value; }
        public double Bit_17 { get => Neurons[17]; set => Neurons[17] = value; }
        public double Bit_18 { get => Neurons[18]; set => Neurons[18] = value; }
        public double Bit_19 { get => Neurons[19]; set => Neurons[19] = value; }
        public double Bit_20 { get => Neurons[20]; set => Neurons[20] = value; }
        public double Bit_21 { get => Neurons[21]; set => Neurons[21] = value; }
        public double Bit_22 { get => Neurons[22]; set => Neurons[22] = value; }
        public double Bit_23 { get => Neurons[23]; set => Neurons[23] = value; }
        public double Bit_24 { get => Neurons[24]; set => Neurons[24] = value; }
        public double Bit_25 { get => Neurons[25]; set => Neurons[25] = value; }
        public double Bit_26 { get => Neurons[26]; set => Neurons[26] = value; }
        public double Bit_27 { get => Neurons[27]; set => Neurons[27] = value; }
        public double Bit_28 { get => Neurons[28]; set => Neurons[28] = value; }
        public double Bit_29 { get => Neurons[29]; set => Neurons[29] = value; }
        public double Bit_30 { get => Neurons[30]; set => Neurons[30] = value; }
        public double Bit_31 { get => Neurons[31]; set => Neurons[31] = value; }
        public double Bit_32 { get => Neurons[32]; set => Neurons[32] = value; }
        public double Bit_33 { get => Neurons[33]; set => Neurons[33] = value; }
        public double Bit_34 { get => Neurons[34]; set => Neurons[34] = value; }
        public double Bit_35 { get => Neurons[35]; set => Neurons[35] = value; }
        public double Bit_36 { get => Neurons[36]; set => Neurons[36] = value; }
        public double Bit_37 { get => Neurons[37]; set => Neurons[37] = value; }
        public double Bit_38 { get => Neurons[38]; set => Neurons[38] = value; }
        public double Bit_39 { get => Neurons[39]; set => Neurons[39] = value; }
        public double Bit_40 { get => Neurons[40]; set => Neurons[40] = value; }
        public double Bit_41 { get => Neurons[41]; set => Neurons[41] = value; }
        public double Bit_42 { get => Neurons[42]; set => Neurons[42] = value; }
        public double Bit_43 { get => Neurons[43]; set => Neurons[43] = value; }
        public double Bit_44 { get => Neurons[44]; set => Neurons[44] = value; }
        public double Bit_45 { get => Neurons[45]; set => Neurons[45] = value; }
        public double Bit_46 { get => Neurons[46]; set => Neurons[46] = value; }
        public double Bit_47 { get => Neurons[47]; set => Neurons[47] = value; }
        public double Bit_48 { get => Neurons[48]; set => Neurons[48] = value; }
        public double Bit_49 { get => Neurons[49]; set => Neurons[49] = value; }
        public double Bit_50 { get => Neurons[50]; set => Neurons[50] = value; }
        public double Bit_51 { get => Neurons[51]; set => Neurons[51] = value; }
        public double Bit_52 { get => Neurons[52]; set => Neurons[52] = value; }
        public double Bit_53 { get => Neurons[53]; set => Neurons[53] = value; }
        public double Bit_54 { get => Neurons[54]; set => Neurons[54] = value; }
        public double Bit_55 { get => Neurons[55]; set => Neurons[55] = value; }
        public double Bit_56 { get => Neurons[56]; set => Neurons[56] = value; }
        public double Bit_57 { get => Neurons[57]; set => Neurons[57] = value; }
        public double Bit_58 { get => Neurons[58]; set => Neurons[58] = value; }
        public double Bit_59 { get => Neurons[59]; set => Neurons[59] = value; }
        public double Bit_60 { get => Neurons[60]; set => Neurons[60] = value; }
        public double Bit_61 { get => Neurons[61]; set => Neurons[61] = value; }
        public double Bit_62 { get => Neurons[62]; set => Neurons[62] = value; }
        public double Bit_63 { get => Neurons[63]; set => Neurons[63] = value; }
    }
}
