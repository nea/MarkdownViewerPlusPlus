#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
//
// Copyright (c) 2005-2016 empira Software GmbH, Cologne Area (Germany)
//
// http://www.pdfsharp.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Diagnostics;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.IO;

namespace PdfSharp.Pdf
{
    /// <summary>
    /// Base class of all composite PDF objects.
    /// </summary>
    public abstract class PdfObject : PdfItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfObject"/> class.
        /// </summary>
        protected PdfObject()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfObject"/> class.
        /// </summary>
        protected PdfObject(PdfDocument document)
        {
            // Calling a virtual member in a constructor is dangerous.
            // In PDFsharp Document is overridden in PdfPage and the code is checked to be save
            // when called for a not completely initialized object.
            Document = document;
        }

        /// <summary>
        /// Initializes a new instance from an existing object. Used for object type transformation.
        /// </summary>
        protected PdfObject(PdfObject obj)
            : this(obj.Owner)
        {
            // If the object that was transformed to an instance of a derived class was an indirect object
            // set the value of the reference to this.
            if (obj._iref != null)
                obj._iref.Value = this;
#if DEBUG_  // BUG
            else
            {
                // If this occurs it is an internal error
                Debug.Assert(false, "Object type transformation must not be done with direct objects");
            }
#endif
        }

        /// <summary>
        /// Creates a copy of this object. The clone does not belong to a document, i.e. its owner and its iref are null.
        /// </summary>
        public new PdfObject Clone()
        {
            return (PdfObject)Copy();
        }

        /// <summary>
        /// Implements the copy mechanism. Must be overridden in derived classes.
        /// </summary>
        protected override object Copy()
        {
            PdfObject obj = (PdfObject)base.Copy();
            obj._document = null;
            obj._iref = null;
            return obj;
        }

#if true_  // works, but may lead to other problems that I cannot assess
        /// <summary>
        /// Determines whether the specified object is equal to the current PdfObject.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is PdfObject)
            {
                PdfObject other = (PdfObject)obj;
                // Take object type transformation into account
                if (_iref != null && other._iref != null)
                {
                    Debug.Assert(_iref.Value != null, "iref without value.");
                    Debug.Assert(other.iref.Value != null, "iref without value.");
                    return Object.ReferenceEquals(_iref.Value, other.iref.Value);
                }
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            if (_iref != null)
            {
                Debug.Assert(_iref.Value != null, "iref without value.");
                return _iref.GetHashCode();
            }
            return base.GetHashCode();
        }
#endif

        /// <summary>
        /// Sets the object and generation number.
        /// Setting the object identifier makes this object an indirect object, i.e. the object gets
        /// a PdfReference entry in the PdfReferenceTable.
        /// </summary>
        internal void SetObjectID(int objectNumber, int generationNumber)
        {
            PdfObjectID objectID = new PdfObjectID(objectNumber, generationNumber);

            // TODO: check imported
            if (_iref == null)
                _iref = _document._irefTable[objectID];
            if (_iref == null)
            {
                // ReSharper disable once ObjectCreationAsStatement because the new object is set to this object
                // in the constructor of PdfReference.
                new PdfReference(this);
                Debug.Assert(_iref != null);
                _iref.ObjectID = objectID;
            }
            _iref.Value = this;
            _iref.Document = _document;
        }

        /// <summary>
        /// Gets the PdfDocument this object belongs to.
        /// </summary>
        public virtual PdfDocument Owner
        {
            get { return _document; }
        }

        /// <summary>
        /// Sets the PdfDocument this object belongs to.
        /// </summary>
        internal virtual PdfDocument Document
        {
            set
            {
                if (!ReferenceEquals(_document, value))
                {
                    if (_document != null)
                        throw new InvalidOperationException("Cannot change document.");
                    _document = value;
                    if (_iref != null)
                        _iref.Document = value;
                }
            }
        }
        internal PdfDocument _document;

        /// <summary>
        /// Indicates whether the object is an indirect object.
        /// </summary>
        public bool IsIndirect
        {
            // An object is an indirect object if and only if is has an indirect reference value.
            get { return _iref != null; }
        }

        /// <summary>
        /// Gets the PdfInternals object of this document, that grants access to some internal structures
        /// which are not part of the public interface of PdfDocument.
        /// </summary>
        public PdfObjectInternals Internals
        {
            get { return _internals ?? (_internals = new PdfObjectInternals(this)); }
        }
        PdfObjectInternals _internals;

        /// <summary>
        /// When overridden in a derived class, prepares the object to get saved.
        /// </summary>
        internal virtual void PrepareForSave()
        { }

        /// <summary>
        /// Saves the stream position. 2nd Edition.
        /// </summary>
        internal override void WriteObject(PdfWriter writer)
        {
            Debug.Assert(false, "Must not come here!");
            //Debug.Assert(_inStreamOffset <= 0);
            //if (_inStreamOffset == 0)
            //{
            //    //_InStreamOffset = stream.Position;
            //    _document.xrefTable.AddObject(this);
            //    return Format("{0} {1} obj\n", _objectID, _generation);
            //}
            //else if (_inStreamOffset == -1)
            //{
            //}
            //return null;
        }

        /// <summary>
        /// Gets the object identifier. Returns PdfObjectID.Empty for direct objects,
        /// i.e. never returns null.
        /// </summary>
        internal PdfObjectID ObjectID
        {
            get { return _iref != null ? _iref.ObjectID : PdfObjectID.Empty; }
        }

        /// <summary>
        /// Gets the object number.
        /// </summary>
        internal int ObjectNumber
        {
            get { return ObjectID.ObjectNumber; }
        }

        /// <summary>
        /// Gets the generation number.
        /// </summary>
        internal int GenerationNumber
        {
            get { return ObjectID.GenerationNumber; }
        }

        ///// <summary>
        ///// Creates a deep copy of the specified value and its transitive closure and adds the
        ///// new objects to the specified owner document.
        ///// </summary>
        /// <param name="owner">The document that owns the cloned objects.</param>
        /// <param name="externalObject">The root object to be cloned.</param>
        /// <returns>The clone of the root object</returns>
        internal static PdfObject DeepCopyClosure(PdfDocument owner, PdfObject externalObject)
        {
            // Get transitive closure.
            PdfObject[] elements = externalObject.Owner.Internals.GetClosure(externalObject);
            int count = elements.Length;
#if DEBUG_
            for (int idx = 0; idx < count; idx++)
            {
                Debug.Assert(elements[idx].XRef != null);
                Debug.Assert(elements[idx].XRef.Document != null);
                Debug.Assert(elements[idx].Document != null);
                if (elements[idx].ObjectID.ObjectNumber == 12)
                    GetType();
            }
#endif
            // 1st loop. Replace all objects by their clones.
            PdfImportedObjectTable iot = new PdfImportedObjectTable(owner, externalObject.Owner);
            for (int idx = 0; idx < count; idx++)
            {
                PdfObject obj = elements[idx];
                PdfObject clone = obj.Clone();
                Debug.Assert(clone.Reference == null);
                clone.Document = owner;
                if (obj.Reference != null)
                {
                    // Case: The cloned object was an indirect object.
                    // Add clone to new owner document.
                    owner._irefTable.Add(clone);
                    // The clone gets an iref by adding it to its new owner.
                    Debug.Assert(clone.Reference != null);
                    // Save an association from old object identifier to new iref.
                    iot.Add(obj.ObjectID, clone.Reference);
                }
                else
                {
                    // Case: The cloned object was an direct object.
                    // Only the root object can be a direct object.
                    Debug.Assert(idx == 0);
                }
                // Replace external object by its clone.
                elements[idx] = clone;
            }
#if DEBUG_
            for (int idx = 0; idx < count; idx++)
            {
                Debug.Assert(elements[idx]._iref != null);
                Debug.Assert(elements[idx]._iref.Document != null);
                Debug.Assert(resources[idx].Document != null);
                if (elements[idx].ObjectID.ObjectNumber == 12)
                    GetType();
            }
#endif

            // 2nd loop. Fix up all indirect references that still refers to the import document.
            for (int idx = 0; idx < count; idx++)
            {
                PdfObject obj = elements[idx];
                Debug.Assert(obj.Owner == owner);
                FixUpObject(iot, owner, obj);
            }

            // Return the clone of the former root object.
            return elements[0];
        }

        ///// <summary>
        ///// Imports an object and its transitive closure to the specified document.
        ///// </summary>
        /// <param name="importedObjectTable">The imported object table of the owner for the external document.</param>
        /// <param name="owner">The document that owns the cloned objects.</param>
        /// <param name="externalObject">The root object to be cloned.</param>
        /// <returns>The clone of the root object</returns>
        internal static PdfObject ImportClosure(PdfImportedObjectTable importedObjectTable, PdfDocument owner, PdfObject externalObject)
        {
            Debug.Assert(ReferenceEquals(importedObjectTable.Owner, owner), "importedObjectTable does not belong to the owner.");
            Debug.Assert(ReferenceEquals(importedObjectTable.ExternalDocument, externalObject.Owner),
                "The ExternalDocument of the importedObjectTable does not belong to the owner of object to be imported.");

            // Get transitive closure of external object.
            PdfObject[] elements = externalObject.Owner.Internals.GetClosure(externalObject);
            int count = elements.Length;
#if DEBUG_
            for (int idx = 0; idx < count; idx++)
            {
                Debug.Assert(elements[idx].XRef != null);
                Debug.Assert(elements[idx].XRef.Document != null);
                Debug.Assert(elements[idx].Document != null);
                if (elements[idx].ObjectID.ObjectNumber == 12)
                    GetType();
            }
#endif
            // 1st loop. Already imported objects are reused and new ones are cloned.
            for (int idx = 0; idx < count; idx++)
            {
                PdfObject obj = elements[idx];
                Debug.Assert(!ReferenceEquals(obj.Owner, owner));

                if (importedObjectTable.Contains(obj.ObjectID))
                {
#if DEBUG_
                    if (obj.ObjectID.ObjectNumber == 5894)
                        obj.GetType();
#endif
                    // Case: External object was already imported.
                    PdfReference iref = importedObjectTable[obj.ObjectID];
                    Debug.Assert(iref != null);
                    Debug.Assert(iref.Value != null);
                    Debug.Assert(iref.Document == owner);
                    // Replace external object by the already cloned counterpart.
                    elements[idx] = iref.Value;
                }
                else
                {
                    // Case: External object was not yet imported ealier and must be cloned.
                    PdfObject clone = obj.Clone();
                    Debug.Assert(clone.Reference == null);
                    clone.Document = owner;
                    if (obj.Reference != null)
                    {
                        // Case: The cloned object was an indirect object.
                        // Add clone to new owner document.
                        owner._irefTable.Add(clone);
                        Debug.Assert(clone.Reference != null);
                        // Save an association from old object identifier to new iref.
                        importedObjectTable.Add(obj.ObjectID, clone.Reference);
                    }
                    else
                    {
                        // Case: The cloned object was a direct object.
                        // Only the root object can be a direct object.
                        Debug.Assert(idx == 0);
                    }
                    // Replace external object by its clone.
                    elements[idx] = clone;
                }
            }
#if DEBUG_
            for (int idx = 0; idx < count; idx++)
            {
                //Debug.Assert(elements[idx].Reference != null);
                //Debug.Assert(elements[idx].Reference.Document != null);
                Debug.Assert(elements[idx].IsIndirect == false);
                Debug.Assert(elements[idx].Owner != null);
                //if (elements[idx].ObjectID.ObjectNumber == 12)
                //    GetType();
            }
#endif
            // 2nd loop. Fix up indirect references that still refers to the external document.
            for (int idx = 0; idx < count; idx++)
            {
                PdfObject obj = elements[idx];
                Debug.Assert(owner != null);
                FixUpObject(importedObjectTable, importedObjectTable.Owner, obj);
            }

            // Return the imported root object.
            return elements[0];
        }

        /// <summary>
        /// Replace all indirect references to external objects by their cloned counterparts
        /// owned by the importer document.
        /// </summary>
        static void FixUpObject(PdfImportedObjectTable iot, PdfDocument owner, PdfObject value)
        {
            Debug.Assert(ReferenceEquals(iot.Owner, owner));

            PdfDictionary dict;
            PdfArray array;
            if ((dict = value as PdfDictionary) != null)
            {
                // Case: The object is a dictionary.
                // Set document for cloned direct objects.
                if (dict.Owner == null)
                {
                    // If the dictionary has not yet an owner set the owner to the importing document.
                    dict.Document = owner;
                }
                else
                {
                    // If the dictionary already has an owner it must be the importing document.
                    Debug.Assert(dict.Owner == owner);
                }

                // Search for indirect references in all dictionary elements.
                PdfName[] names = dict.Elements.KeyNames;
                foreach (PdfName name in names)
                {
                    PdfItem item = dict.Elements[name];
                    Debug.Assert(item != null, "A dictionary element cannot be null.");

                    // Is item an iref?
                    PdfReference iref = item as PdfReference;
                    if (iref != null)
                    {
                        // Case: The item is a reference.
                        // Does the iref already belongs to the new owner?
                        if (iref.Document == owner)
                        {
                            // Yes: fine. Happens when an already cloned object is reused.
                            continue;
                        }

                        //Debug.Assert(iref.Document == iot.Document);
                        // No: Replace with iref of cloned object.
                        PdfReference newXRef = iot[iref.ObjectID];  // TODO: Explain this line of code in all details.
                        Debug.Assert(newXRef != null);
                        Debug.Assert(newXRef.Document == owner);
                        dict.Elements[name] = newXRef;
                    }
                    else
                    {
                        // Case: The item is not a reference.
                        // If item is an object recursively fix its inner items.
                        PdfObject pdfObject = item as PdfObject;
                        if (pdfObject != null)
                        {
                            // Fix up inner objects, i.e. recursively walk down the object tree.
                            FixUpObject(iot, owner, pdfObject);
                        }
                        else
                        {
                            // The item is something else, e.g. a name.
                            // Nothing to do.

                            // ...but let's double check this case in DEBUG build.
                            DebugCheckNonObjects(item);
                        }
                    }
                }
            }
            else if ((array = value as PdfArray) != null)
            {
                // Case: The object is an array.
                // Set document for cloned direct objects.
                if (array.Owner == null)
                {
                    // If the array has not yet an owner set the owner to the importing document.
                    array.Document = owner;
                }
                else
                {
                    // If the array already has an owner it must be the importing document.
                    Debug.Assert(array.Owner == owner);
                }

                // Search for indirect references in all array elements.
                int count = array.Elements.Count;
                for (int idx = 0; idx < count; idx++)
                {
                    PdfItem item = array.Elements[idx];
                    Debug.Assert(item != null, "An array element cannot be null.");

                    // Is item an iref?
                    PdfReference iref = item as PdfReference;
                    if (iref != null)
                    {
                        // Case: The item is a reference.
                        // Does the iref already belongs to the owner?
                        if (iref.Document == owner)
                        {
                            // Yes: fine. Happens when an already cloned object is reused.
                            continue;
                        }

                        // No: replace with iref of cloned object.
                        Debug.Assert(iref.Document == iot.ExternalDocument);
                        PdfReference newXRef = iot[iref.ObjectID];
                        Debug.Assert(newXRef != null);
                        Debug.Assert(newXRef.Document == owner);
                        array.Elements[idx] = newXRef;
                    }
                    else
                    {
                        // Case: The item is not a reference.
                        // If item is an object recursively fix its inner items.
                        PdfObject pdfObject = item as PdfObject;
                        if (pdfObject != null)
                        {
                            // Fix up inner objects, i.e. recursively walk down the object tree.
                            FixUpObject(iot, owner, pdfObject);
                        }
                        else
                        {
                            // The item is something else, e.g. a name.
                            // Nothing to do.

                            // ...but let's double check this case in DEBUG build.
                            DebugCheckNonObjects(item);
                        }
                    }
                }
            }
            else
            {
                // Case: The item is some other indirect object.
                // Indirect integers, booleans, etc. are allowed, but PDFsharp do not create them.
                // If such objects occur in imported PDF files from other producers, nothing more is to do.
                // The owner was already set, which is double checked by the assertions below.
                if (value is PdfNameObject || value is PdfStringObject || value is PdfBooleanObject || value is PdfIntegerObject || value is PdfNumberObject)
                {
                    Debug.Assert(value.IsIndirect);
                    Debug.Assert(value.Owner == owner);
                }
                else
                    Debug.Assert(false, "Should not come here. Object is neither a dictionary nor an array.");
            }
        }

        /// <summary>
        /// Ensure for future versions of PDFsharp not to forget code for a new kind of PdfItem.
        /// </summary>
        /// <param name="item">The item.</param>
        [Conditional("DEBUG")]
        static void DebugCheckNonObjects(PdfItem item)
        {
            if (item is PdfName)
                return;
            if (item is PdfBoolean)
                return;
            if (item is PdfInteger)
                return;
            if (item is PdfNumber)
                return;
            if (item is PdfString)
                return;
            if (item is PdfRectangle)
                return;
            if (item is PdfNull)
                return;

            Type type = item.GetType();
            Debug.Assert(type != null, string.Format("CheckNonObjects: Add {0} to the list.", type.Name));
        }

        /// <summary>
        /// Gets the indirect reference of this object. If the value is null, this object is a direct object.
        /// </summary>
        public PdfReference Reference
        {
            get { return _iref; }

            // Setting the reference outside PDFsharp is not considered as a valid operation.
            internal set { _iref = value; }
        }
        PdfReference _iref;
    }
}