﻿using Cosmos.Core.Memory;
using Cosmos.HAL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShardOS
{
    public static class CobaltCore
    {
        static int LastHeap;
        public static int _frames;
        public static int _fps = 200;
        public static int _deltaT = 0;
        public static int _maximalfps = 0;
        public static void CallCore()
        {
            CallHeapCollect();
            CountFPS();
        }
        public static void CallHeapCollect()
        {
            if (LastHeap >= 4)
            {
                LastHeap = 0;
                Heap.Collect();
            }
            else
                LastHeap++;
        }
        public static void CountFPS()
        {
            if (_deltaT != RTC.Second)
            {
                _fps = _frames;
                _frames = 0;
                _deltaT = RTC.Second;
                if(_fps > _maximalfps)
                {
                    _maximalfps = _fps;
                }
                BitmapDraws.Clear(Desktop.fps, Color.Black);
                BitmapDraws.DrawString(Desktop.fps, CobaltCore._fps.ToString() + ", max recorded " + CobaltCore._maximalfps.ToString(), 0, 0, Color.Green);
            }
            _frames++;
        }
    }
}