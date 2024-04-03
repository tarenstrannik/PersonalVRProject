using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class XRSocketInteractableMultiselect : XRSocketInteractor
{
    private Dictionary<XRGrabInteractable, GrabInteractableProperties> m_multiselectedObjects = new Dictionary<XRGrabInteractable, GrabInteractableProperties>();

    [SerializeField] private UnityEvent<SelectEnterEventArgs> m_onAddToSelection;
    [SerializeField] private UnityEvent<SelectExitEventArgs> m_onRemoveFromSelection;

    private class GrabInteractableProperties
    {
        public bool IsSelected = false;
        public Vector3 LocalPosition= Vector3.zero;
        public Quaternion LocalRotation = Quaternion.identity;
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        var obj = args.interactableObject as XRGrabInteractable;

        var objProperties = new GrabInteractableProperties();
        

        m_multiselectedObjects.TryAdd(obj, objProperties);
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        var obj = args.interactableObject as XRGrabInteractable;
        if (m_multiselectedObjects.ContainsKey(obj))
            m_multiselectedObjects.Remove(obj);
    }

    protected virtual void AddToSelection(XRGrabInteractable grabInteractable)
    {

        if (m_multiselectedObjects.ContainsKey(grabInteractable))
            m_multiselectedObjects[grabInteractable].IsSelected = true;

        Vector3 localPosition = transform.InverseTransformVector(grabInteractable.transform.position);
        Quaternion localRotation = Quaternion.Inverse(transform.rotation) * grabInteractable.transform.rotation;

        m_multiselectedObjects[grabInteractable].LocalPosition = localPosition;
        m_multiselectedObjects[grabInteractable].LocalRotation = localRotation;

        var args = new SelectEnterEventArgs();
        args.interactorObject = this;
        args.interactableObject = grabInteractable;

        m_onAddToSelection.Invoke(args);
    }


    protected virtual void RemoveFromSelection(XRGrabInteractable grabInteractable)
    {
        if (m_multiselectedObjects.ContainsKey(grabInteractable))
            m_multiselectedObjects[grabInteractable].IsSelected = false;

        m_multiselectedObjects[grabInteractable].LocalPosition = Vector3.zero;
        m_multiselectedObjects[grabInteractable].LocalRotation = Quaternion.identity;

        var args = new SelectExitEventArgs();
        args.interactorObject = this;
        args.interactableObject = grabInteractable;

        m_onRemoveFromSelection.Invoke(args);
    }

    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        return false;
    }

    public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractor(updatePhase);
        foreach (var obj in m_multiselectedObjects)
        {
            if (ObjectWasAddedToSelection(obj.Key, obj.Value.IsSelected))
            {
                AddToSelection(obj.Key);
            }

            else if(ObjectWasRemovedSelection(obj.Key, obj.Value.IsSelected))
            {
                RemoveFromSelection(obj.Key);
            }
                

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed)
            {
                if (obj.Key == true)
                    ProcessSelectedObject(obj.Key);
            }
        }
        

    }
    private bool ObjectWasAddedToSelection(XRGrabInteractable grabInteractable, bool wasPreviouslySelected)
    {

        if (grabInteractable.interactorsSelecting.Count > 0 && wasPreviouslySelected == false)
        {
            return true;
        }
        return false;
    }
    private bool ObjectWasRemovedSelection(XRGrabInteractable grabInteractable, bool wasPreviouslySelected)
    {

        if (grabInteractable.interactorsSelecting.Count == 0 && wasPreviouslySelected == true)
        {
            return true;
        }
        return false;
    }

    private void ProcessSelectedObject(XRGrabInteractable grabInteractable)
    {
        grabInteractable.transform.position = transform.TransformVector(m_multiselectedObjects[grabInteractable].LocalPosition);
        grabInteractable.transform.rotation = transform.rotation * m_multiselectedObjects[grabInteractable].LocalRotation;
    }
}
