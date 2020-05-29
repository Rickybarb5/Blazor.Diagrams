﻿using Blazor.Diagrams.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Blazor.Diagrams.Core
{
    public class DiagramManager
    {
        private readonly List<Node> _nodes;
        private bool _mouseDownOnNode;

        public event Action<Node> NodeAdded;
        public event Action<Node> NodeRemoved;

        public DiagramManager()
        {
            _nodes = new List<Node>();
        }

        public ReadOnlyCollection<Node> Nodes => _nodes.AsReadOnly();
        public IEnumerable<Link> AllLinks => _nodes.SelectMany(n => n.Ports.SelectMany(p => p.Links));
        public Node SelectedNode { get; private set; }

        public void AddNode(Node node)
        {
            _nodes.Add(node);
            NodeAdded?.Invoke(node);
        }

        public void RemoveNode(Node node)
        {
            if (_nodes.Remove(node))
            {
                NodeRemoved?.Invoke(node);
            }
        }

        public void OnMouseDown()
        {
            _mouseDownOnNode = false;
            SelectedNode.Selected = false;
            SelectedNode.Refresh();
            SelectedNode = null;
        }

        public void OnMouseDown(Node node, double[] offsets, double clientX, double clientY)
        {
            node.Offset = new Point(offsets[0] - clientX, offsets[1] - clientY);
            SelectNode(node);
            _mouseDownOnNode = true;
        }

        public void OnMouseMove(double clientX, double clientY)
        {
            if (!_mouseDownOnNode)
                return;

            SelectedNode.UpdatePosition(clientX, clientY);
            SelectedNode.RefreshAll();
        }

        public void OnMouseUp()
        {
            _mouseDownOnNode = false;
        }

        public void SelectNode(Node node)
        {
            if (SelectedNode == node)
                return;

            if (SelectedNode != null)
            {
                SelectedNode.Selected = false;
                SelectedNode.Refresh();
            }

            SelectedNode = node;
            SelectedNode.Selected = true;
        }
    }
}
