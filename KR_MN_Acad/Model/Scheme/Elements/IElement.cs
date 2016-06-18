﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.Scheme.Materials;
using KR_MN_Acad.Scheme.Spec;

namespace KR_MN_Acad.Scheme.Elements
{
    /// <summary>
    /// Элемент спецификации материалов (Поз, Обозн, Наимен, Кол, Масса ед, Примечание)
    /// </summary>
    public interface IElement : IMaterial, IComparable<IElement>, IEquatable<IElement>
    {
        /// <summary>
        /// Позиция элемента взятая из блока.
        /// Заполненная юзером или при нумермации
        /// </summary>
        string PositionInBlock { get; set; }
        string Prefix { get; set; }
        ISpecRow SpecRow { get; set; }
        GroupType Type { get; set; }         
        ISchemeBlock Block { get; set; }
        /// <summary>
        /// Человеческое название элемента в блоке - верхняя арматура, шпилька и т.п.
        /// </summary>
        string FriendlyName { get; set; }

        /// <summary>
        /// Расчет матариала (массы)
        /// </summary>
        void Calc();
        /// <summary>
        /// Суммирование элементов и заполнение строк:
        /// Обозначения, Наименования, Кол, Массы ед, примечания
        /// Результат записать в SpecRow
        /// </summary>        
        void Sum(List<IElement> elems);
        /// <summary>
        /// Строка элемента для заполнения в выноску в блоке
        /// </summary>
        /// <returns></returns>
        string GetDesc();
        /// <summary>
        /// Присвоение позиции элементов (после группировки)
        /// </summary>
        /// <param name="posIndex">Индекс позиции по порядку</param>
        /// <param name="items">Элементы в строке спецификации</param>
        /// <param name="isNumbering">True - при нуменрации, False - при спецификации</param>
        string GetPosition (int posIndex, IEnumerable<IElement> items, bool isNumbering);
        /// <summary>
        /// При необходимости - сортировка строк в спецификации
        /// </summary>
        /// <param name="rows">Строки спецификации - одной группы элементов</param>
        void SortRowsSpec (List<ISpecRow> rows);
    }        
}
