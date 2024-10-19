// Authors: Layla Hoey
namespace SystemMiami
{
    public class Resource
    {
        private float _max;
        private float _current;

        // Constructors
        public Resource()
        {
            _max = 0;
            _current = 0;
        }

        public Resource(float max)
        {
            _max = max;
            _current = _max;
        }

        // Getters
        public float Get()
        {
            return _current;
        }

        // Setters
        public void Reset()
        {
            _current = _max;
        }

        /// <summary>
        /// Gain an amount. If the new value
        /// is greater than the max, set the val to max
        /// </summary>
        public void Gain(float amt)
        {
            float newVal = _current + amt;

            _current = newVal > _max ? _max : newVal;
        }

        /// <summary>
        /// Lose an amount. If the new value
        /// is less than 0, set the val to 0
        /// </summary>
        public void Lose(float amt)
        {
            float newVal = _current - amt;

            _current = newVal < 0 ? 0 : newVal;
        }
    }
}
