//
// RadixTree.cs
//
// Author:
//       John Mark Gabriel Caguicla <caguicla.jmg@hapticbunnystudios.com>
//
// Copyright (c) 2018 Copyright © 2018 John Mark Gabriel Caguicla
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;

namespace WanaKanaSharp.Utility
{
	public class Trie<TKey, TValue>
	{
		public class Node : IEnumerable<Node>
		{
			class Enumerator : IEnumerator<Node>
			{
				Dictionary<TKey, Node>.Enumerator DictionaryEnumerator;

				public Node Current => DictionaryEnumerator.Current.Value;

				object IEnumerator.Current => Current;

				public Enumerator(Dictionary<TKey, Node>.Enumerator enumerator)
				{
					DictionaryEnumerator = enumerator;
				}

				public void Dispose()
				{
					DictionaryEnumerator.Dispose();
				}

				public bool MoveNext() => DictionaryEnumerator.MoveNext();

				public void Reset() => throw new NotImplementedException();
			}

			public TKey Key { get; private set; }
			public Node Parent { get; private set; }
			public TValue Value { get; set; }

			Dictionary<TKey, Node> Children = new Dictionary<TKey, Node>();

			public Node(TKey key, TValue value)
			{
				Key = key;
				Parent = null;
				Value = value;
			}

			public Node this[TKey key] => Children[key];

			public Boolean ContainsKey(TKey key) => Children.ContainsKey(key);

			public Node Duplicate(Boolean copyChildren = false)
			{
				var node = new Node(Key, Value);

				if (copyChildren)
				{
					foreach (var child in this)
					{
						var c = child.Duplicate(copyChildren);
						node.Insert(c);
					}
				}

				return node;
			}

			public Node GetChild(TKey key)
			{
				if (Children.TryGetValue(key, out Node node)) return node;

				return null;
			}

			public IEnumerator<Node> GetEnumerator()
			{
				return new Enumerator(Children.GetEnumerator());
			}

			public Node Insert(Node child)
			{
				child.Parent = this;
				Children.Add(child.Key, child);
				return child;
			}

			public void Insert(params Node[] children)
			{
				foreach (var child in children)
				{
					Insert(child);
				}
			}

			public Node Insert((TKey Key, TValue Value) child)
			{
				var node = new Node(child.Key, child.Value);
				return Insert(node);
			}

			public void Insert(params (TKey Key, TValue Value)[] children)
			{
				foreach (var child in children)
				{
					Insert(child);
				}
			}

			public void Remove(params TKey[] keys)
			{
				foreach (var key in keys)
				{
					if (!Children.ContainsKey(key)) throw new KeyNotFoundException();

					var node = Children[key];
					node.Key = default(TKey);
					Children.Remove(key);
				}
			}

			public void Traverse(Action<Node> action, Int32 maxDepth = -1)
			{
				Traverse(action, 0, maxDepth);
			}

			public void TraverseChildren(Action<Node> action, Int32 maxDepth = 0)
			{
				foreach (var child in this)
				{
					child.Traverse(action, 0, maxDepth);
				}
			}

			void Traverse(Action<Node> action, Int32 currentDepth, Int32 maxDepth)
			{
				action(this);

				if (currentDepth == maxDepth) return;

				foreach (var child in this)
				{
					child.Traverse(action, currentDepth + 1, maxDepth);
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		public Node this[TKey key]
		{
			get { return Root[key]; }
		}

		public Node Root { get; } = new Node(default(TKey), default(TValue));

		public void Merge(Trie<TKey, TValue> trie)
		{
			trie.Root.TraverseChildren((node) =>
			{
				Root.Insert(node.Duplicate(true));
			});
		}

		public static Trie<TKey, TValue> Merge(Trie<TKey, TValue> a, Trie<TKey, TValue> b)
		{
			var trie = new Trie<TKey, TValue>();
			var root = trie.Root;

			a.Root.TraverseChildren((node) =>
			{
				root.Insert(node.Duplicate(true));
			});

			b.Root.TraverseChildren((node) =>
			{
				root.Insert(node.Duplicate(true));
			});

			return trie;
		}
	}
}
