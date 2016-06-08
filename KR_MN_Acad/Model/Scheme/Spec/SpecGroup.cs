﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.Scheme.Elements;

namespace KR_MN_Acad.Scheme.Spec
{
    public enum GroupType
    {
        Unknown,
        /// <summary>
        /// Стержни
        /// </summary>
        Armatures,
        /// <summary>
        /// Детали
        /// </summary>
        Details,
        /// <summary>
        /// Закладные детали
        /// </summary>
        EmbeddedDetails,
        /// <summary>
        /// Материалы
        /// </summary>
        Materials
    }    

    /// <summary>
    /// Группа в спецификации материалов схемы армирования
    /// </summary>
    public class SpecGroup
    {
        public bool HasPosition { get; set; } = true;
        /// <summary>
        /// Имя группы
        /// </summary>
        public GroupType Type { get; set; }
        /// <summary>
        /// Елементы группы
        /// </summary>
        public List<ISpecRow> Rows { get; set; }
        public string Name { get; set; }
        public List<IElement> Elements { get; set; }

        public SpecGroup(IGrouping<GroupType, IElement> elems)
        {
            Type = elems.Key;
            GetGroupName(Type);
            Elements = elems.ToList();
        }

        private void GetGroupName(GroupType type)
        {
            switch (type)
            {
                case GroupType.Unknown:
                    Name = "";
                    break;
                case GroupType.Armatures:
                    Name = "Стержни";
                    break;
                case GroupType.Details:
                    Name = "Детали";
                    break;
                case GroupType.EmbeddedDetails:
                    Name = "Закладные детали";
                    break;
                case GroupType.Materials:
                    Name = "Материалы";
                    HasPosition = false;
                    break;
                default:
                    Name = "";
                    break;
            }
        }

        /// <summary>
        /// Калькуляция группы - сортировка по элементам и назначение позиций
        /// </summary>
        public void Calculate(bool isNumbering)
        {
            Rows = new List<ISpecRow>();
            // Группировка элементов по типам
            IOrderedEnumerable<IGrouping<IElement, IElement>> someElems;
            if (isNumbering)
            {
                // При нумерации - сортировка по элементам
                someElems = Elements.GroupBy(g => g).OrderBy(o => o.Key);
            }
            else
            {
                // Без нумерации - сортировка по позициям
                someElems = Elements.GroupBy(g => g).OrderBy(o => o.Key.PositionInBlock);
            }
            int posIndex = 0;
            string prefix = someElems.First().Key.Prefix;
            foreach (var item in someElems)
            {
                if (string.Equals(prefix, item.Key.Prefix))
                {
                    posIndex++;
                }
                else
                {
                    prefix = item.Key.Prefix;
                    posIndex = 1;
                }
                // Составить строчку таблицы
                string pos = string.Empty;
                if (HasPosition)
                {
                    if (isNumbering)
                    {
                        pos = item.Key.Prefix + posIndex;
                    }
                    else
                    {
                        pos = item.Key.PositionInBlock;
                    }
                }                
                ISpecRow row = new SpecRow(pos , item.ToList());
                row.Calculate();
                Rows.Add(row);                
            }
        }
    }
}
