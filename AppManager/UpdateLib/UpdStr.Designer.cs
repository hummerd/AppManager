﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4016
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UpdateLib {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class UpdStr {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal UpdStr() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("UpdateLib.UpdStr", typeof(UpdStr).Assembly);
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
        ///   Looks up a localized string similar to Do you want to install {0} new version {1}?.
        /// </summary>
        internal static string ASK_FOR_INSTALL {
            get {
                return ResourceManager.GetString("ASK_FOR_INSTALL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Downloading.
        /// </summary>
        internal static string DOWNLOADING {
            get {
                return ResourceManager.GetString("DOWNLOADING", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New version {0} of {1} is avialable at
        ///{2}.
        ///{3}
        ///Do you want to download it?.
        /// </summary>
        internal static string NEW_VER_AVIALABLE {
            get {
                return ResourceManager.GetString("NEW_VER_AVIALABLE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Updater.
        /// </summary>
        internal static string UPDATER {
            get {
                return ResourceManager.GetString("UPDATER", resourceCulture);
            }
        }
    }
}
