using Captura;
using SharpAvi.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalMacroRecorder
{
    public class TimedFrameWriter
    {
        public TimedFrameWriter(IAviVideoStream videoStream, AviWriter writer, RecorderParams @params)
        {
            VideoStream = videoStream;
            Writer = writer;
            Params = @params;
            msPerFrame = (1.0 / (double)writer.FramesPerSecond) * 1000;
        }

        public IAviVideoStream VideoStream { get; }
        public AviWriter Writer { get; }
        public RecorderParams Params { get; }

        private double msPerFrame;

        public void Close()
        {
            VideoStream.WriteFrame(true, lastBuffer, 0, lastBuffer.Length);
            Writer.Close();
        }

        DateTime lastWrite = DateTime.MinValue;
        DateTime nextWrite = DateTime.MinValue;
        byte[] lastBuffer = null;
        internal void WriteFrame(bool keyFrame, byte[] buffer, int start, int length)
        {
            if (lastWrite != DateTime.MinValue)
            {
                var now = DateTime.Now;
                //lastwrite=00:00,nextwrite=00:00.250

                //now:00:00.240, copy buffer do nothing
                //now:00:00.250, copy buffer, write last buffer nothing
                //now:00:00.499, copy buffer, write last buffer nothing
                //now:00:00.501, copy buffer, write last buffer twice
                while (now > nextWrite)
                {
                    VideoStream.WriteFrame(keyFrame, lastBuffer, start, length);
                    lastWrite = nextWrite;
                    nextWrite = lastWrite.AddMilliseconds(msPerFrame);
                }
            }
            else
            {
                lastWrite = DateTime.Now;
                nextWrite = lastWrite.AddMilliseconds(msPerFrame);
                VideoStream.WriteFrame(keyFrame, buffer, start, length);
            }
            lastBuffer = buffer.ToArray();
        }
    }
}
