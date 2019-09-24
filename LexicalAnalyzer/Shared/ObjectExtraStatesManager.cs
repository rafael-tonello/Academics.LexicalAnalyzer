using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NShared
{

    /// <summary>
    /// Sometimes, we work with some object that do not have a property that we would really like it to have. Often, if an objecthad too much property,
    /// it would prevent us from having to write several lines of code.
    /// 
    /// This library server to meet this need. It allows properties to be associated with objects.
    /// 
    /// It does't add properties to objects, but maintains an association between any property (which we ourselves invented) with an object of interest
    /// </summary>
    public class StateManager
    {
        //this delegate is used when a property associated to an object is observated
        public delegate void SMObjserver(VarValue newValue);

        //this dictionary maintains a association of properties with objects
        private static Dictionary<object, Dictionary<object, VarValue>> ObjectsStates = new Dictionary<object, Dictionary<object, VarValue>>();

        /// <summary>
        /// Associate a property with an object
        /// </summary>
        /// <param name="target">The target object</param>
        /// <param name="property">The name of the property. Usually is a string</param>
        /// <param name="value">The value to be associated with 'target' as 'property'</param>
        public static void set(object target, object property, object value)
        {

            if (!ObjectsStates.ContainsKey(target))
                ObjectsStates[target] = new Dictionary<object, VarValue>();

            if (!ObjectsStates[target].ContainsKey(property))
                ObjectsStates[target][property] = new VarValue();

            ObjectsStates[target][property].AsRaw = value;

            __notifyObservers(target, property, ObjectsStates[target][property]);
        }

        /// <summary>
        /// Gets a property associated to the 'target'
        /// </summary>
        /// <param name="target">The target object</param>
        /// <param name="property">The name of the property. Usually is a string</param>
        /// <param name="defaultValue">The value to be returned if the 'target' does't have a 'property'</param>
        /// <returns>return on VarValue object. This object can return value as int, string, double, raw value or a forced type (with getT<T> method)</returns>
        public static VarValue get(object target, object property, object defaultValue)
        {
            if (!ObjectsStates.ContainsKey(target))
                return new VarValue { AsRaw = defaultValue };

            if (!ObjectsStates[target].ContainsKey(property))
                return new VarValue { AsRaw = defaultValue };

            return (ObjectsStates[target][property]);
        }
        /// <summary>
        /// Returns a forced type object. If the property has a different type than 'T', this function will throw an Exception
        /// </summary>
        /// <typeparam name="T">The of return</typeparam>
        /// <param name="target">The target object</param>
        /// <param name="property">The name of the property. Usually is a string</param>
        /// <param name="defaultValue">The value to be returned if the 'target' does't have a 'property'</param>
        /// <returns>The desired object</returns>
        public static T get<T>(object target, object property, T defaultValue)
        {
            if (!ObjectsStates.ContainsKey(target))
            {
                return defaultValue;
            }

            if (!ObjectsStates[target].ContainsKey(property))
                return defaultValue;

            return ObjectsStates[target][property].getT<T>();
        }

        /// <summary>
        /// Deletes a property associated to the 'target'
        /// </summary>
        /// <param name="target">The target object</param>
        /// <param name="property">The name of the property. Usually is a string</param>
        /// <returns>return on VarValue object. This object can return value as int, string, double, raw value or a forced type (with getT<T> method)</returns>
        public static bool del(object target, object property)
        {
            if (!ObjectsStates.ContainsKey(target))
                return false;

            if (!ObjectsStates[target].ContainsKey(property))
                return false;

            ObjectsStates[target][property] = null;
            ObjectsStates[target].Remove(property);
            return true;
        }

        /// <summary>
        /// Observe an associated property with an object.
        /// </summary>
        /// <param name="target">The target object</param>
        /// <param name="property">The name of the property. Usually is a string</param>
        /// <param name="action">The action to be executed every time that 'property' is changed</param>
        public static void observe(object target, object property, SMObjserver action)
        {
            if (!ObjectsStates.ContainsKey(target))
                ObjectsStates[target] = new Dictionary<object, VarValue>();

            if (!ObjectsStates[target].ContainsKey("__observers__"))
            {
                ObjectsStates[target]["__observers__"] = new VarValue
                {
                    AsRaw = new Dictionary<object, List<SMObjserver>>()
                };
            }

            Dictionary<object, List<SMObjserver>> observers = (Dictionary<object, List<SMObjserver>>)ObjectsStates[target]["__observers__"].AsRaw;

            if (!observers.ContainsKey(property))
                observers[property] = new List<SMObjserver>();

            observers[property].Add(action);
        }

        private static void __notifyObservers(object target, object property, VarValue newValue)
        {

            if (ObjectsStates.ContainsKey(target))
            {

                if (ObjectsStates[target].ContainsKey("__observers__"))
                {


                    Dictionary<object, List<SMObjserver>> observers = (Dictionary<object, List<SMObjserver>>)ObjectsStates[target]["__observers__"].AsRaw;

                    if (observers.ContainsKey(property))
                    {
                        for (int c = 0; c < observers[property].Count; c++)
                            observers[property][c](newValue);
                    }
                }
            }
        }
    }

    public class SS : StateManager { }
    public class SM : StateManager { }
    public class Tag : StateManager { }
    public class Tags : StateManager { }
    public class ObjectVars : StateManager { }



    public class VarValue
    {
        object _value;
        
        public object AsRaw
        {
            get { return _value; }
            set { _value = value; }
        }

        public void setT<T>(T obj)
        {
            _value = obj;
        }

        public T getT<T>()
        {
            return (T)_value;
        }

        public string AsString
        {
            get { return _value is string ? (string)_value : _value.ToString(); }
            set { _value = value; }
        }

        public int AsInt
        {
            get
            {
                int ret = 0;
                if (!int.TryParse(AsString, out ret))
                    ret = 0;

                return ret;
            }
            set { _value = value.ToString(); }
        }

        public double AsDouble
        {
            get
            {
                string tempValue = AsString.Replace(".", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator).Replace(",", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);
                double ret = 0.0;
                if (!double.TryParse(tempValue, out ret))
                    ret = 0.0;

                return ret;
            }
            set { _value = value.ToString(); }
        }

        public bool AsBoolean
        {
            get
            {
                if (AsString.ToLower() == "true")
                    return true;
                else
                    return false;
            }
            set { _value = value.ToString(); }
        }

        public DateTime AsDateTime
        {
            get
            {
                DateTime ret;
                if (DateTime.TryParse(AsString, out ret))
                    return ret;
                else
                    return new DateTime(0, 1, 1, 0, 0, 0);
                //return DateTime.Parse("01/01/0001 00:00:00");
            }
            set
            {
                _value = value.ToString();
            }
        }
    }
}
