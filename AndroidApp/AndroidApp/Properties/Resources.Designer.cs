﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AndroidApp.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("AndroidApp.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;html&gt;
        ///&lt;body&gt;
        ///
        ///    &lt;h1&gt;Headers we got:&lt;/h1&gt;
        ///    &lt;pre&gt;
        ///{0}
        ///    &lt;/pre&gt;
        ///
        ///    &lt;h1&gt;Body we got:&lt;/h1&gt;
        ///    &lt;pre&gt;
        ///{1}
        ///    &lt;/pre&gt;
        ///
        ///    &lt;h1&gt;Method: &lt;b&gt;{2}&lt;/b&gt;&lt;/h1&gt;
        ///
        ///    &lt;h1&gt;Path: &lt;b&gt;{3}&lt;/b&gt;&lt;/h1&gt;
        ///
        ///
        ///    &lt;br /&gt;
        ///    &lt;form method=&quot;post&quot;&gt;
        ///        &lt;input type=&quot;hidden&quot; name=&quot;a&quot; value=&quot;im hidden&quot; /&gt;
        ///        &lt;input type=&quot;submit&quot; value=&quot;Send this!&quot; /&gt;
        ///    &lt;/form&gt;
        ///
        ///    &lt;script&gt;
        ///        function alertPost(url, bodyString) {
        ///            fetch(url, {
        ///                method: &quot;POST&quot;,
        ///                header [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Echo {
            get {
                return ResourceManager.GetString("Echo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;html&gt;
        ///&lt;head&gt;
        ///    &lt;title&gt;{title}&lt;/title&gt;
        ///&lt;/head&gt;
        ///&lt;body&gt;
        ///    {body}
        ///&lt;/body&gt;
        ///&lt;/html&gt;.
        /// </summary>
        internal static string EmptyHTML {
            get {
                return ResourceManager.GetString("EmptyHTML", resourceCulture);
            }
        }
    }
}
