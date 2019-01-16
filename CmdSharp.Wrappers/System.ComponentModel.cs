using System.Runtime;

namespace System.ComponentModel
{
    //
    // Summary:
    //     Encapsulates zero or more components.
    public class Container : IContainer, IDisposable
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Container class.
        public Container() { }

        //
        // Summary:
        //     Releases unmanaged resources and performs other cleanup operations before the
        //     System.ComponentModel.Container is reclaimed by garbage collection.

        //
        // Summary:
        //     Gets all the components in the System.ComponentModel.Container.
        //
        // Returns:
        //     A collection that contains the components in the System.ComponentModel.Container.
        public virtual ComponentCollection Components { get; }

        //
        // Summary:
        //     Adds the specified System.ComponentModel.Component to the System.ComponentModel.Container.
        //     The component is unnamed.
        //
        // Parameters:
        //   component:
        //     The component to add.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     component is null.
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public virtual void Add(IComponent component) { }
        //
        // Summary:
        //     Adds the specified System.ComponentModel.Component to the System.ComponentModel.Container
        //     and assigns it a name.
        //
        // Parameters:
        //   component:
        //     The component to add.
        //
        //   name:
        //     The unique, case-insensitive name to assign to the component.-or- null, which
        //     leaves the component unnamed.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     component is null.
        //
        //   T:System.ArgumentException:
        //     name is not unique.
        public virtual void Add(IComponent component, string name) { }
        //
        // Summary:
        //     Releases all resources used by the System.ComponentModel.Container.
        public void Dispose() { }
        //
        // Summary:
        //     Removes a component from the System.ComponentModel.Container.
        //
        // Parameters:
        //   component:
        //     The component to remove.
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public virtual void Remove(IComponent component) { }
        //
        // Summary:
        //     Creates a site System.ComponentModel.ISite for the given System.ComponentModel.IComponent
        //     and assigns the given name to the site.
        //
        // Parameters:
        //   component:
        //     The System.ComponentModel.IComponent to create a site for.
        //
        //   name:
        //     The name to assign to component, or null to skip the name assignment.
        //
        // Returns:
        //     The newly created site.
        protected virtual ISite CreateSite(IComponent component, string name) { return null;  }
        //
        // Summary:
        //     Releases the unmanaged resources used by the System.ComponentModel.Container,
        //     and optionally releases the managed resources.
        //
        // Parameters:
        //   disposing:
        //     true to release both managed and unmanaged resources; false to release only unmanaged
        //     resources.
        protected virtual void Dispose(bool disposing) { }
        //
        // Summary:
        //     Gets the service object of the specified type, if it is available.
        //
        // Parameters:
        //   service:
        //     The System.Type of the service to retrieve.
        //
        // Returns:
        //     An System.Object implementing the requested service, or null if the service cannot
        //     be resolved.
        protected virtual object GetService(Type service) { return null; }
        //
        // Summary:
        //     Removes a component from the System.ComponentModel.Container without setting
        //     System.ComponentModel.IComponent.Site to null.
        //
        // Parameters:
        //   component:
        //     The component to remove.
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        protected void RemoveWithoutUnsiting(IComponent component) { }
        //
        // Summary:
        //     Determines whether the component name is unique for this container.
        //
        // Parameters:
        //   component:
        //     The named component.
        //
        //   name:
        //     The component name to validate.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     component is null.
        //
        //   T:System.ArgumentException:
        //     name is not unique.
        protected virtual void ValidateName(IComponent component, string name) { }
    }
}
