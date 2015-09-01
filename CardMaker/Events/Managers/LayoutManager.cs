﻿////////////////////////////////////////////////////////////////////////////////
// The MIT License (MIT)
//
// Copyright (c) 2015 Tim Stair
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
////////////////////////////////////////////////////////////////////////////////


using CardMaker.Card;
using CardMaker.Forms;
using CardMaker.XML;

namespace CardMaker.Events.Managers
{
    /// <summary>
    /// Handles Layout related communication between components
    /// </summary>
    public class LayoutManager
    {
        private static LayoutManager m_zLayoutManager;
        
        public Deck ActiveDeck { get; private set; }
        public ProjectLayout ActiveLayout { get; private set; }

        /// <summary>
        /// Fired when a layout is loaded (deck is loaded)
        /// </summary>
        public event LayoutLoaded LayoutLoaded;

        /// <summary>
        /// Fired when a layout is altered (including when a child element is altered)
        /// </summary>
        public event LayoutUpdated LayoutUpdated;

        /// <summary>
        /// Fired when the order of the elements is requested
        /// </summary>
        public event LayoutElementOrderRequest ElementOrderAdjustRequest;

        /// <summary>
        /// Fired when a selected element change is requested
        /// </summary>
        public event LayoutElementSelectRequest ElementSelectAdjustRequest;

        /// <summary>
        /// Fired when the deck index changes
        /// </summary>
        public event DeckIndexChanged DeckIndexChanged;

        public static LayoutManager Instance
        {
            get
            {
                if (m_zLayoutManager == null)
                {
                    m_zLayoutManager = new LayoutManager();
                }
                return m_zLayoutManager;
            }
        }

        private LayoutManager()
        {
            // where best to listen for project selection and then deck loading
            MDIProject.Instance.LayoutSelected += LayoutSelected;
        }

        void LayoutSelected(object sender, LayoutEventArgs args)
        {
            // TODO: load some stuff
            ActiveLayout = args.Layout;
            InitializeActiveLayout();
        }

        /// <summary>
        /// Refreshes the Deck assocaited with the current Layout
        /// </summary>
        public void InitializeActiveLayout()
        {
            if (null != ActiveLayout)
            {
                ActiveDeck = new Deck();

                ActiveDeck.SetAndLoadLayout(ActiveLayout, false);
            }

            if (null != LayoutLoaded)
            {
                LayoutLoaded(this, new LayoutEventArgs(ActiveLayout, ActiveDeck));
            }            
        }

        /// <summary>
        /// Initialize the cache for each element in the ProjectLayout
        /// </summary>
        /// <param name="zLayout">The layout to initialize the cache</param>
        public static void InitializeElementCache(ProjectLayout zLayout)
        {
            // mark all fields as specified
            if (null != zLayout.Element)
            {
                foreach (var zElement in zLayout.Element)
                {
                    zElement.InitializeCache();
                }
            }
        }

        //TODO: (might even want to include the origin sender)
        /// <summary>
        /// This is the event for a modification to the layout/elements requiring save and re-draw
        /// </summary>
        public void FireLayoutUpdatedEvent()
        {
            if (null != LayoutUpdated)
            {
                LayoutUpdated(this, new LayoutEventArgs(ActiveLayout, ActiveDeck));
            }
        }

        //TODO: proper name (might even want to include the origin sender)
        /// <summary>
        /// Adjusts the active card inex in the Deck
        /// </summary>
        /// <param name="nDesiredIndex">The target index</param>
        public void SetDeckIndex(int nDesiredIndex)
        {
            // TODO: error check
            ActiveDeck.CardIndex = nDesiredIndex;
            if (null != DeckIndexChanged)
            {
                DeckIndexChanged(this, new DeckChangeEventArgs(ActiveDeck) );
            }
        }

        // TODO: this "request" event should make the actual change and then fire the event
        public void RequestElementAdjustOrder(int nIndexAdjust)
        {
            if (null != ElementOrderAdjustRequest)
            {
                ElementOrderAdjustRequest(this, new LayoutElementNumericAdjustEventArgs(nIndexAdjust));
            }
        }

        // TODO: this "request" event should ... dunno... how to tell the layout control window to perform a task?
        // might need to go through the element manager instead when selection is managed(??)
        public void RequestSelectedElementIndexAdjust(int nAdjust)
        {
            if (null != ElementSelectAdjustRequest)
            {
                ElementSelectAdjustRequest(this, new LayoutElementNumericAdjustEventArgs(nAdjust));
            }
        }
    }
}