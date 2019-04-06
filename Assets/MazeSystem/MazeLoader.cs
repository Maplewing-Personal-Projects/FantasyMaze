using System;
using System.IO;
using System.Linq;

namespace Maplewing.MazeSystem
{
    public class MazeLoader
    {
        private string _path;
        
        public MazeLoader(string path)
        {
            _path = path;
        }

        public MazeSystem.MapObject[,] LoadMap()
        {
            var text = File.ReadAllText(_path);
            var lines = text.Split('\n').Select(line => line.Trim()).ToArray();

            var map = new MazeSystem.MapObject[lines.Length, lines[0].Length];
            for(int i = 0; i < lines.Length; ++i)
            {
                for(int j = 0; j < lines[i].Length; ++j)
                {
                    map[i, j] = _GetObject(lines[i][j]);
                }
            }

            return map;
        }

        private MazeSystem.MapObject _GetObject(char objectSymbol)
        {
            switch (objectSymbol)
            {
                case 'E': return MazeSystem.MapObject.Empty;
                case 'W': return MazeSystem.MapObject.Wall;
                case 'S': return MazeSystem.MapObject.Start;
                case 'X': return MazeSystem.MapObject.Exit;
                default: throw new Exception("Object not found!");
            }
        }
    }
}