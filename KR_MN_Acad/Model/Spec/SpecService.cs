﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using AcadLib.Jigs;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace KR_MN_Acad.Spec
{
    /// <summary>
    /// Спецификация блоков в различные таблицы
    /// </summary>
    public class SpecService
    {
        Document doc;
        Database db;
        Editor ed;
        ISpecOptions options;

        public SpecService(Document doc, ISpecOptions options)
        {
            this.doc = doc;
            ed = doc.Editor;
            db = doc.Database;
            this.options = options;
        }

        /// <summary>
        /// Нумерация блоков
        /// </summary>
        public void Numbering ()
        {
            // Выбор блоков
            var sel = SelectBlocks();
            // Определение блоков схемы армирования
            var blocks = FilterBlocks(sel);
            // Проверка правил расположение блоков
            //CheckRules();               
            // Калькуляция всех элементов            
            options.TableService.Numbering(blocks);
            NumberingBlocks(blocks);
        }       

        /// <summary>
        /// Создание спецификации
        /// </summary>
        public void Spec ()
        {
            // Выбор блоков
            var sel = SelectBlocks();
            // Определение блоков схемы армирования
            var blocks = FilterBlocks(sel);
            // Проверка правил расположение блоков
            //CheckRules();
            // Калькуляция всех элементов            
            options.TableService.CalcRows(blocks);
            // Проверка позиций
            //CheckPositions();
            // Создание спецификаций.
            var table = options.TableService.CreateTable();
            InsertTable(table);        
        }        

        /// <summary>
        /// Выбор блоков
        /// </summary>
        /// <returns></returns>
        private List<ObjectId> SelectBlocks ()
        {
            return ed.SelectBlRefs("\nВыбор блоков спецификации:");
        }

        /// <summary>
        /// Отбор и определение блоков спецификации
        /// </summary>        
        private List<ISpecBlock> FilterBlocks (List<ObjectId> ids)
        {
            var blocks = new List<ISpecBlock>();            
            if (ids == null || ids.Count == 0) return blocks;            
            using (var t = db.TransactionManager.StartTransaction())
            {
                foreach (var idEnt in ids)
                {
                    var blRef = idEnt.GetObject(OpenMode.ForRead, false, true) as BlockReference;
                    if (blRef == null) continue;
                    string blName = blRef.GetEffectiveName();
                    ISpecBlock block = SpecBlockFactory.CreateBlock(blRef, blName, options);
                    if (block == null)
                    {
                        ed.WriteMessage($"\nПропущен блок '{blName}'");
                        continue;
                    }
                    block.Calculate();
                    if (block.Error == null)
                        blocks.Add(block);
                    else
                        Inspector.Errors.Add(block.Error);
                }
                t.Commit();
            }
            if (blocks.Count == 0)
            {
                throw new Exception($"\nБлоки для спецификации не определены.");
            }

            // Проверка дубликатов
            if (options.CheckDublicates)
            {
                AcadLib.Blocks.Dublicate.CheckDublicateBlocks.Check(blocks.Select(s=>s.Block.IdBlRef));
            }

            return blocks;
        }

        private void NumberingBlocks (List<ISpecBlock> blocks)
        {
            // Заполнение позиций в блоках
            using (var t = db.TransactionManager.StartTransaction())
            {
                foreach (var item in blocks)
                {
                    item.Numbering();
                }
                t.Commit();
            }
        }

        private void InsertTable (Table table)
        {
            using (var t = db.TransactionManager.StartTransaction())
            {
                var cs = db.CurrentSpaceId.GetObject( OpenMode.ForWrite) as BlockTableRecord;
                List<ObjectId> idsDrag = new List<ObjectId> ();
                var scale = AcadLib.Scale.ScaleHelper.GetCurrentAnnoScale(db);

                table.TransformBy(Matrix3d.Scaling(scale, table.Position));
                cs.AppendEntity(table);
                t.AddNewlyCreatedDBObject(table, true);
                idsDrag.Add(table.Id);

                if (!DragSel.Drag(ed, idsDrag.ToArray(), Point3d.Origin))
                {
                    foreach (var id in idsDrag)
                    {
                        var ent = id.GetObject( OpenMode.ForWrite);
                        ent.Erase();
                    }
                }
                t.Commit();
            }                
        }
    }
}
