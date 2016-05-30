﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AcadLib.Errors;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using KR_MN_Acad.Model.Pile.Numbering;
using KR_MN_Acad.Spec;
using SpecBlocks;

[assembly: CommandClass(typeof(KR_MN_Acad.Commands))]
[assembly: ExtensionApplication(typeof(KR_MN_Acad.Commands))]

namespace KR_MN_Acad
{
    public class Commands: IExtensionApplication
    {
        const string groupPik = AutoCAD_PIK_Manager.Commands.Group;                       

        public void Initialize()
        {
            AcadLib.LoadService.LoadSpecBlocks();
            AcadLib.LoadService.LoadNetTopologySuite();
        }

        /// <summary>
        /// Спецификация монолитных блоков
        /// </summary>
        [CommandMethod(groupPik, nameof(KR_SpecMonolith), CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void KR_SpecMonolith()
        {
            AcadLib.CommandStart.Start(doc =>
            {                               
                // Спецификация монолитных блоков                         
                SpecService specService = new SpecService(new SpecMonolith());
                specService.CreateSpec();                
            });            
        }

        /// <summary>
        /// Спецификация проемов - Дверных и Оконных
        /// </summary>
        [CommandMethod(groupPik, nameof(KR_SpecApertures), CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void KR_SpecApertures()
        {
            AcadLib.CommandStart.Start(doc =>
            {                
                // Спецификация проекмов
                SpecService specService = new SpecService(new SpecAperture());
                specService.CreateSpec();             
            });                
        }

        /// <summary>
        /// Спецификация отверстий в стене
        /// </summary>
        [CommandMethod(groupPik, nameof(KR_SpecOpenings), CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void KR_SpecOpenings()
        {
            AcadLib.CommandStart.Start(doc =>
            {                
                // Спецификация отверстий
                SpecService specService = new SpecService(new SpecOpenings());
                specService.CreateSpec();                
            });            
        }

        /// <summary>
        /// Спецификация отверстий в плите
        /// </summary>
        [CommandMethod(groupPik, nameof(KR_SpecSlabOpenings), CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void KR_SpecSlabOpenings()
        {
            AcadLib.CommandStart.Start(doc =>
            {                
                // Спецификация отверстий
                SpecService specService = new SpecService(new SpecSlabOpenings());
                specService.CreateSpec();                
            });
        }

        /// <summary>
        /// Нумерация блоков монолита
        /// </summary>
        [CommandMethod(groupPik, nameof(KR_MonolithNumbering), CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void KR_MonolithNumbering()
        {
            AcadLib.CommandStart.Start(doc =>
            {                
                SpecService specService = new SpecService(new SpecMonolith());
                specService.Numbering();                
            });
        }

        /// <summary>
        /// Нумерация блоков проемов
        /// </summary>
        [CommandMethod(groupPik, nameof(KR_AperturesNumbering), CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void KR_AperturesNumbering()
        {
            AcadLib.CommandStart.Start(doc =>
            {                
                SpecService specService = new SpecService(new SpecAperture());
                specService.Numbering();                
            });
        }

        /// <summary>
        /// Нумерация блоков отверстий в стене
        /// </summary>
        [CommandMethod(groupPik,nameof(KR_OpeningsNumbering), CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void KR_OpeningsNumbering()
        {
            AcadLib.CommandStart.Start(doc =>
            {                
                SpecService specService = new SpecService(new SpecOpenings());
                specService.Numbering();                
            });
        }

        /// <summary>
        /// Нумерация блоков отверстий в плите
        /// </summary>
        [CommandMethod(groupPik, nameof(KR_SlabOpeningsNumbering), CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void KR_SlabOpeningsNumbering()
        {
            AcadLib.CommandStart.Start(doc =>
            {                
                SpecService specService = new SpecService(new SpecSlabOpenings());
                specService.Numbering();                
            });
        }        

        /// <summary>
        /// Нумерация свай
        /// </summary>
        [CommandMethod(groupPik,nameof(KR_PileNumbering), CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void KR_PileNumbering()
        {
            AcadLib.CommandStart.Start(doc =>
            {                
                PileNumberingService pileNumbService = new PileNumberingService();
                pileNumbService.Numbering();
                doc.Editor.WriteMessage("\nНумерация выполнена.");                
            });           
        }

        /// <summary>
        /// Параметры свай
        /// </summary>
        [CommandMethod(groupPik,nameof(KR_PileOptions), CommandFlags.Modal)]
        public void KR_PileOptions()
        {
            AcadLib.CommandStart.Start(doc =>
            {
                var pileOpt = Model.Pile.PileOptions.Load();
                pileOpt = pileOpt.PromptOptions();
                pileOpt.Save();
                doc.Editor.WriteMessage("\nНастойки сохранены.");
            });            
        }

        /// <summary>
        /// Спецификация свай
        /// </summary>
        [CommandMethod(groupPik,nameof(KR_PileCalc), CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void KR_PileCalc()
        {
            AcadLib.CommandStart.Start(doc =>
            {                
                // Выбор свай для нумерации
                var selblocks = doc.Editor.SelectBlRefs("\nВыбор блоков свай:");
                // фильтр блоков свай            
                var piles = Model.Pile.PileFilter.Filter(selblocks, Model.Pile.PileOptions.Load(), true);
                // Проверка дубликатов
                AcadLib.Blocks.Dublicate.CheckDublicateBlocks.Check(piles.Select(p => p.IdBlRef));
                // Расчет свай
                Model.Pile.Calc.PileCalcService pileCalcService = new Model.Pile.Calc.PileCalcService();
                pileCalcService.Calc(piles);                
            });            
        }

        public void Terminate()
        {
        }
    }
}