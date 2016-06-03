﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.Scheme;

namespace KR_MN_Acad.ConstructionServices.Materials
{
    /// <summary>
    /// Распределенные стержни на участке
    /// </summary>
    public class ArmatureDivision
    {
        /// <summary>
        /// Стержень с Count
        /// </summary>
        public Armature Armature { get; set; }
        /// <summary>
        /// Ширина распределения
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Шаг распределения
        /// </summary>
        public int Step { get; set; }                

        public ArmatureDivision (int diam, int length, int width, int step)
        {
            Width = width;
            Step = step;            
            Armature = new Armature(diam, length);
            CalcCount();            
        }        

        private void CalcCount()
        {
            // Кол стержней
            int count = Width / Step + 1;
            Armature.Count = count;                     
        }        

        public string GetDesc()
        {
            return $"{Armature.GetLeaderDesc()}, шаг {Step}";
        }
    }
}