using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyIdentication.DAL.Entities
{
    public class ElasticSearch
    {
        /// <summary>
        /// Gets or sets
        /// <para>ID</para>
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets
        /// <para>URI</para>
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets
        /// <para>IndexName</para>
        /// </summary>
        public string IndexName { get; set; }

        /// <summary>
        /// Gets or sets
        /// <para>TypeName</para>
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or sets
        /// <para>UserName</para>
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets
        /// <para>Password</para>
        /// </summary>
        public string Password { get; set; }
    }
}