using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackExchange.Redis.Provider.Core.Configuration
{
    public class RedisHostCollection : ConfigurationElementCollection
    {
        
		/// <summary>
		/// Gets or sets the <see cref="RedisHost"/> at the specified index.
		/// </summary>
		/// <value>
		/// The <see cref="RedisHost"/>.
		/// </value>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public RedisHost this[int index]
		{
			get
			{
				return BaseGet(index) as RedisHost;
			}
			set
			{
				if (BaseGet(index) != null)
				{
					BaseRemoveAt(index);
				}

				BaseAdd(index, value);
			}
		}

		/// <summary>
		/// Creates the new element.
		/// </summary>
		/// <returns></returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new RedisHost();
		}

        /// <summary>
        /// Gets the element key.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return string.Format("{0}:{1}", ((RedisHost)element).Host, ((RedisHost)element).CachePort);
        }


    }
}
