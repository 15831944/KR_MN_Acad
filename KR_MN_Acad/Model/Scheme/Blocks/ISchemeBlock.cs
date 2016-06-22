﻿using System.Collections.Generic;
using AcadLib.Blocks;
using AcadLib.Errors;
using AcadLib.RTree.SpatialIndex;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Scheme.Elements;

namespace KR_MN_Acad.Scheme
{
    /// <summary>
    /// Блок для схемы армирования
    /// </summary>
    public interface ISchemeBlock
    {
        /// <summary>
        /// Имя блока
        /// </summary>
        string BlName { get; set; }        
        ObjectId IdBlref { get; set; }
        Error Error { get; set; }
        Extents3d Extents { get; set; }
        Rectangle Rectangle { get; set; }
        /// <summary>
        /// Все свойства блока - дин и атрибуты
        /// </summary>
        Dictionary<string, Property> Properties { get; set; }
        /// <summary>
        /// Определение материалов блока по свойствам.
        /// Исключения нужно обрабатывать и заносить в Error
        /// </summary>
        void Calculate ();
        /// <summary>
        /// Получение всех материалов блока
        /// </summary>
        /// <returns></returns>
        List<IElement> GetElements();
        /// <summary>
        /// Нумерация элементов блока и заполнение выносок.
        /// </summary>
        void Numbering();
    }
}