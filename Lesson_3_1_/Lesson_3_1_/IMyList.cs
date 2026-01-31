namespace Lesson_3_1_
{
    public interface IMyList
    {
        public bool Add(int num);
        public bool RemoveAll(int num);
        public bool Remove(int num);
        public bool Contains(int num);
        public int IndexOf(int num);
        public bool RemoveAt(int index);
        public int GetById(int index);
        public void DisplayElements();
    }
}