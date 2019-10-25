﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace otor.msixhero.lib.BusinessLayer.Actions
{
    public enum SelectionMode
    {
        SelectAll,
        UnselectAll,
        AddToSelection,
        RemoveFromSelection,
        ReplaceSelection
    }

    [Serializable]
    public class SelectPackages : BaseAction
    {
        public SelectPackages()
        {
            this.Selection = new List<Package>();
            this.SelectionMode = SelectionMode.ReplaceSelection;
        }

        public SelectPackages(List<Package> selection, SelectionMode selectionMode = SelectionMode.ReplaceSelection)
        {
            this.Selection = selection;
            this.SelectionMode = selectionMode;
        }

        public SelectPackages(Package selection, SelectionMode selectionMode = SelectionMode.ReplaceSelection)
        {
            this.Selection = new List<Package> { selection };
            this.SelectionMode = selectionMode;
        }

        public SelectPackages(IEnumerable<Package> selection, SelectionMode selectionMode = SelectionMode.ReplaceSelection)
        {
            this.Selection = new List<Package>(selection);
            this.SelectionMode = selectionMode;
        }

        public SelectPackages(params Package[] selection) : this(selection.ToList(), SelectionMode.ReplaceSelection)
        {
        }

        public List<Package> Selection { get; set; }

        public SelectionMode SelectionMode { get; set; }

        public static SelectPackages CreateSingle(Package singleSelection)
        {
            return new SelectPackages(singleSelection);
        }

        public static SelectPackages CreateAddition(Package addition)
        {
            return new SelectPackages(new[] { addition }, SelectionMode.AddToSelection);
        }

        public static SelectPackages CreateAddition(params Package[] addition)
        {
            return new SelectPackages(addition, SelectionMode.AddToSelection);
        }

        public static SelectPackages CreateSubtraction(Package addition)
        {
            return new SelectPackages(new[] { addition }, SelectionMode.RemoveFromSelection);
        }

        public static SelectPackages CreateSubtraction(params Package[] addition)
        {
            return new SelectPackages(addition, SelectionMode.RemoveFromSelection);
        }

        public static SelectPackages CreateEmpty()
        {
            return new SelectPackages(new Package[0], SelectionMode.UnselectAll);
        }

        public static SelectPackages CreateAll()
        {
            return new SelectPackages(new Package[0], SelectionMode.SelectAll);
        }
    }
}
