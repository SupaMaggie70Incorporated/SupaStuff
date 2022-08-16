using System;
using System.Collections;
using System.Collections.Generic;

namespace SupaStuff.Util {
  public class SupaQueue<T> : IEnumerable<T> {

    public int capacity {
      get {
        return queue.Length;
      }
    }
    public T[] queue;
    Queue<int> inactive;

    private int _index = 0;
    private int index {
      get {
        return _index;
      }
      set {
        _index = value % capacity;
      }
      
    }
    private int count = 0;
    
    public IEnumerator<T> GetEnumerator()
    {
        while(HasNext()) {
            yield return Next();
        }
        yield break;
    }
    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }


    public SupaQueue(int capacity = 1024) {
      queue = new T[capacity];
      inactive = new Queue<int>(capacity);
    }
    public T Next() {
      if(count == 0) {
        return default(T);
      }
      IncrementIndex();
      count--;
      while(inactive.Contains(index)) {
        if(count == 0) return default(T);
        IncrementIndex();
        count--;
        
      }
      T value = queue[index];
      IncrementIndex();
      count--;
      return value;
      
    }
    public T Peek() {
      if(count == 0) {
        return default(T);
      }
      IncrementIndex();
      count--;
      while(inactive.Contains(index)) {
        if(count == 0) return default(T);
        IncrementIndex();
        count--;
        
      }
      return queue[index];
    }
    public void Add(T value) {
      if(AtLimit()) throw new OverflowException("Tried to add to SupaQueue past limit of " + capacity);
      count++;
      int loc;
      if(inactive.Count != 0) {
        loc = inactive.Dequeue();
      }
      else {
        loc = Add(count,index);
      }
      queue[loc] = value;
    }
    public void AddToStart(T value) {
      if(AtLimit()) throw new OverflowException("Tried to add to SupaQueue past limit of " + capacity);
      DecrementIndex();
      count++;
      queue[index] = value;
    }
    public bool HasNext() {
      return count > 0;
    }
    public void Remove(int index) {
      inactive.Enqueue(index);
    }
    public void Remove(T value) {
      for(int i = index;i != Add(index,count-63);Add(i,1)) {
        if(!inactive.Contains(i) && queue[i].Equals(value)) {
          inactive.Enqueue(i);
        }
      }
    }
    public void RemoveAll(T value) {
      for(int i = 0;i < capacity;i++) {
        if(!inactive.Contains(i) && value.Equals(queue[i])) {
          inactive.Enqueue(i);
        }
      }
    }
    private bool AtLimit() {
      return count == capacity;
    }
    private void IncrementIndex() {
      if(index == capacity -1) index = 0;
      else index++;
    }
    private void DecrementIndex() {
      if(index == 0) index = capacity - 1;
      else index--;
    }
    private int Add(int num1,int num2) {
      int val1 = num1 + num2;
      if(val1 < 0) {
        return val1 + 64;
      }
      else return val1 % capacity;
    }
  }
}