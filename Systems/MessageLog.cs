using RLNET;
using System.Collections.Generic;

namespace DarnDroidz.Core
{
    public class MessageLog
    {
        private const int MaxLines = 9; 
        private readonly Queue<string> _lines;

        public static MessageLog Instance { get; } = new MessageLog();

        private MessageLog()
        {
            _lines = new Queue<string>();
        }

        public void Add(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            _lines.Enqueue(message);

            if (_lines.Count > MaxLines)
            {
                _lines.Dequeue();
            }
        }

        public void Draw(RLConsole console)
        {
            console.Clear();

            string[] lines = _lines.ToArray();

            for (int i = 0; i < lines.Length; i++)
            {
                console.Print(1, i + 1, lines[i], RLColor.White);
            }
        }
    }
}
