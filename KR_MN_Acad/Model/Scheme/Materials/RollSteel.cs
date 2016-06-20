﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.ConstructionServices;
using KR_MN_Acad.Scheme.Spec;

namespace KR_MN_Acad.Scheme.Materials
{
    /// <summary>
    /// Прокатная сталь
    /// </summary>
    public abstract class RollSteel : IMaterial, IBillMaterial
    {   
        public Gost Gost { get; set; }
        public string Name { get; set; }        

        public string BillTitle { get; } = "Изделия закладные, кг";
        public string BillGroup { get; } = "Прокат марки";
        public string BillMark { get;  } = "ВСт3кп";
        public string BillGOST { get { return Gost.Number; } }
        public string BillName { get { return Name; }  }

        /// <summary>
        /// Создание прокатной стали 
        /// </summary>
        /// <param name="gost">ГОСТ</param>
        /// <param name="name">Имя, как в ведомости расхода стали. Например: для трубы - ∅108х3, для полосы - 4х40</param>
        public RollSteel (Gost gost, string name)
        {
            Gost = gost;
            Name = name;                        
        }        
    }
}