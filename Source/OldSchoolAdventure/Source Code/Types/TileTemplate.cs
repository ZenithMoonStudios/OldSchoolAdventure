﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=4.0.30319.1.
// 
namespace OSATypes
{

	/// <remarks/>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]

	[System.Diagnostics.DebuggerStepThroughAttribute()]

	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
	public partial class TileTemplate
	{

		private TileTemplateTileSize tileSizeField;

		private TileTemplateAcceleration accelerationField;

		private TemplateSurface leftField;

		private TemplateSurface topField;

		private TemplateSurface rightField;

		private TemplateSurface bottomField;

		private string frictionField;

		private string offenseField;

		private string isDeadlyField;

		private string defenseField;

		private string compensateForGravityChangesField;

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("TileSize", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
		public TileTemplateTileSize TileSize
		{
			get
			{
				return this.tileSizeField;
			}
			set
			{
				this.tileSizeField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("Acceleration", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
		public TileTemplateAcceleration Acceleration
		{
			get
			{
				return this.accelerationField;
			}
			set
			{
				this.accelerationField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("Left", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
		public TemplateSurface Left
		{
			get
			{
				return this.leftField;
			}
			set
			{
				this.leftField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("Top", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
		public TemplateSurface Top
		{
			get
			{
				return this.topField;
			}
			set
			{
				this.topField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("Right", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
		public TemplateSurface Right
		{
			get
			{
				return this.rightField;
			}
			set
			{
				this.rightField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("Bottom", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
		public TemplateSurface Bottom
		{
			get
			{
				return this.bottomField;
			}
			set
			{
				this.bottomField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string Friction
		{
			get
			{
				return this.frictionField;
			}
			set
			{
				this.frictionField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string Offense
		{
			get
			{
				return this.offenseField;
			}
			set
			{
				this.offenseField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string IsDeadly
		{
			get
			{
				return this.isDeadlyField;
			}
			set
			{
				this.isDeadlyField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string Defense
		{
			get
			{
				return this.defenseField;
			}
			set
			{
				this.defenseField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string CompensateForGravityChanges
		{
			get
			{
				return this.compensateForGravityChangesField;
			}
			set
			{
				this.compensateForGravityChangesField = value;
			}
		}
	}

	/// <remarks/>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]

	[System.Diagnostics.DebuggerStepThroughAttribute()]

	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class TileTemplateTileSize
	{

		private string widthField;

		private string heightField;

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string Width
		{
			get
			{
				return this.widthField;
			}
			set
			{
				this.widthField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string Height
		{
			get
			{
				return this.heightField;
			}
			set
			{
				this.heightField = value;
			}
		}
	}

	/// <remarks/>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]

	[System.Diagnostics.DebuggerStepThroughAttribute()]

	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class TileTemplateAcceleration
	{

		private string xField;

		private string yField;

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string X
		{
			get
			{
				return this.xField;
			}
			set
			{
				this.xField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string Y
		{
			get
			{
				return this.yField;
			}
			set
			{
				this.yField = value;
			}
		}
	}


	/// <remarks/>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]

	[System.Diagnostics.DebuggerStepThroughAttribute()]

	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
	public partial class TileTemplateDataSet
	{

		private TileTemplate[] itemsField;

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("TileTemplate")]
		public TileTemplate[] Items
		{
			get
			{
				return this.itemsField;
			}
			set
			{
				this.itemsField = value;
			}
		}
	}
}