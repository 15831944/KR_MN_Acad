﻿using System;

namespace KR_MN_Acad.Spec.WallOpenings.Elements
{    
    /// <summary>
    /// Гильза в стене
    /// </summary>
    public class WallSleeve: ISpecElement, IOpeningElement
    {
        private double diam;
        private double depth;
        private int length;    

        public string Dimension { get; set; }
        public string Elevation { get; set; }
        public string Mark { get; set; }
        public string Role { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
        public string FriendlyName { get; set; } = "Гильза";
        public int Index { get; set; } = 1;
        public GroupType Group { get; set; } = WallOptions.GroupSleeve;
        public ISpecBlock SpecBlock { get; set; }
        public double Amount { get; set; } = 1;
        public string Key { get; set; }

        public WallSleeve (string mark, double diam, double depth, int length, string elev, string role, string desc, ISpecBlock specBlock)
        {
            SpecBlock = specBlock;
            this.diam = diam;
            this.depth = depth;
            this.length = length;
            Elevation = "Ось отв. " + elev;
            Mark = mark;            
            Role = role;            
            Description = desc;
            Count = 1;
            Dimension = $"Гильза {AcadLib.General.Symbols.Diam}{diam}х{depth}, L={length}";
            Key = Dimension + Elevation + Role;
        }                

        public string GetNumber (string index)
        {
            return "Г" + index;
        }

        public void SetNumber (string num, int indexFirst, int indexSecond)
        {
            Mark = num;
        }

        public bool Equals (ISpecElement other)
        {
            var s = other as WallSleeve;
            if (s == null) return false;
            return Mark == s.Mark && diam == s.diam && depth == s.depth && length == s.length &&
                string.Equals(Role,s.Role, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(Elevation,s.Elevation, StringComparison.OrdinalIgnoreCase);
        }

        public int CompareTo (ISpecElement other)
        {
            var s = other as WallSleeve;
            if (s == null) return -1;
            int res=0;
            if (!string.IsNullOrEmpty(Mark))
                res = TableService.alpha.Compare(Mark, s.Mark);
            if (res != 0) return res;
            res = diam.CompareTo(s.diam);
            if (res != 0) return res;
            res = depth.CompareTo(s.depth);
            if (res != 0) return res;
            res = length.CompareTo(s.length);
            if (res != 0) return res;
            res = Role.CompareTo(s.Role);
            if (res != 0) return res;
            res = AcadLib.Comparers.AlphanumComparator.New.Compare(Elevation, s.Elevation);
            return res;
        }

        public override int GetHashCode ()
        {
            return Dimension.GetHashCode();
        }

        public string GetParamInfo ()
        {
            return $"{Dimension} {Elevation} {Role}";
        }

        public string GetDesc ()
        {
            return "";
        }

        public void Calc ()
        {            
        }
    }
}