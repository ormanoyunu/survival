using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Malee {

	[Serializable]
	public abstract class ReorderableArray<T> : ICloneable, IList<T>, ICollection<T>, IEnumerable<T> {

		public List<T> Array { get => m_Array; set => m_Array = value; }

		[SerializeField]
		private List<T> m_Array = new List<T>();

		public ReorderableArray()
			: this(0) {
		}

		public ReorderableArray(int length) {

			m_Array = new List<T>(length);
		}

		public T this[int index] {

			get { return m_Array[index]; }
			set { m_Array[index] = value; }
		}

		public int Length {

			get { return m_Array.Count; }
		}

		public bool IsReadOnly {

			get { return false; }
		}

		public int Count {

			get { return m_Array.Count; }
		}

		public object Clone() {

			return new List<T>(m_Array);
		}

		public void CopyFrom(IEnumerable<T> value) {

			m_Array.Clear();
			m_Array.AddRange(value);
		}

		public bool Contains(T value) {

			return m_Array.Contains(value);
		}

		public int IndexOf(T value) {

			return m_Array.IndexOf(value);
		}

		public void Insert(int index, T item) {

			m_Array.Insert(index, item);
		}

		public void RemoveAt(int index) {

			m_Array.RemoveAt(index);
		}

		public void Add(T item) {

			m_Array.Add(item);
		}

		public void Clear() {

			m_Array.Clear();
		}

		public void CopyTo(T[] array, int arrayIndex) {

			this.m_Array.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item) {

			return m_Array.Remove(item);
		}

		public T[] ToArray() {

			return m_Array.ToArray();
		}

		public IEnumerator<T> GetEnumerator() {

			return m_Array.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {

			return m_Array.GetEnumerator();
		}
	}
}
