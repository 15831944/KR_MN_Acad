﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.ConstructionServices.Materials;

namespace KR_MN_Acad.Scheme.Spec
{
    /// <summary>
    /// Строчка в спецификации материалов схемы армирования
    /// </summary>
    public class RowScheme : IComparable<RowScheme>, IEquatable<RowScheme>
    {
        public List<IMaterial> Materials { get; set; }
        /// <summary>
        /// Позиция - столбец
        /// </summary>
        public string PositionColumn { get;  set; }
        /// <summary>
        /// Обозначение - столбец
        /// </summary>
        public string DocumentColumn { get;  set; }
        /// <summary>
        /// Наименование - столбец
        /// </summary>
        public string NameColumn { get; set; }
        /// <summary>
        /// Кол.
        /// </summary>
        public string CountColumn { get; set; }
        /// <summary>
        /// Масса ед., кг
        /// </summary>
        public string WeightColumn { get; set; }
        /// <summary>
        /// Примечание
        /// </summary>
        public string DescriptionColumn { get; set; }
        /// <summary>
        /// Группа элемента
        /// </summary>
        public GroupType Type { get; set; }   
        /// <summary>
        /// Префикс позиции - используется только для группировки и сравнения строк.
        /// </summary>
        public string PositionPrefix { get; set; }

        public RowScheme(GroupType type, string posPrefix)
        {
            PositionPrefix = posPrefix;
            Type = type;
        }

        public int CompareTo(RowScheme other)
        {
            var result = Type.CompareTo(other.Type);
            if (result != 0) return result;

            result = string.Compare(PositionPrefix, other.PositionPrefix, true);
            if (result != 0) return result;

            result = string.Compare(DocumentColumn,other.DocumentColumn, true);
            if (result != 0) return result;

            result = string.Compare(NameColumn, other.NameColumn, true);
            if (result != 0) return result;

            return 0;            
        }

        public void SetPosition(int pos)
        {
            PositionColumn = Materials.First().GetPosition(pos);
            foreach (var item in Materials)
            {
                item.Position = this;
            }
        }

        public bool Equals(RowScheme other)
        {
            return this.CompareTo(other) == 0;
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode() ^ PositionPrefix.GetHashCode();
        }
    }
}
