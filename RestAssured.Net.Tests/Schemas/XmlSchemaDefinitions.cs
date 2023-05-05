// <copyright file="XmlSchemaDefinitions.cs" company="On Test Automation">
// Copyright 2019 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
namespace RestAssured.Tests.Schemas
{
    /// <summary>
    /// A class containing XML schema definitions (XSDs) used in the tests.
    /// </summary>
    public class XmlSchemaDefinitions
    {
        /// <summary>
        /// An XML schema that matches the XML payload used in the tests.
        /// </summary>
        internal static string MatchingXmlSchemaAsString { get; } = @"<?xml version=""1.0"" encoding=""utf-8""?>
            <xs:schema xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"" attributeFormDefault=""unqualified"" elementFormDefault=""qualified"">
              <xsd:element name=""Location"">
                <xsd:complexType>
                  <xsd:sequence>
                    <xsd:element name=""Country"" type=""xsd:string"" />
                    <xsd:element name=""State"" type=""xsd:string"" />
                    <xsd:element name=""ZipCode"" type=""xsd:unsignedInt"" />
                    <xsd:element name=""Places"">
                      <xsd:complexType>
                        <xsd:sequence>
                          <xsd:element maxOccurs=""unbounded"" name=""Place"">
                            <xsd:complexType>
                              <xsd:sequence>
                                <xsd:element name=""Name"" type=""xsd:string"" />
                                <xsd:element name=""Inhabitants"" type=""xsd:unsignedInt"" />
                                <xsd:element name=""IsCapital"" type=""xsd:boolean"" />
                              </xsd:sequence>
                            </xsd:complexType>
                          </xsd:element>
                        </xsd:sequence>
                      </xsd:complexType>
                    </xsd:element>
                  </xsd:sequence>
                </xsd:complexType>
              </xsd:element>
            </xs:schema>";

        /// <summary>
        /// An XML schema that does not match the XML payload used in the tests.
        /// </summary>
        internal static string NonMatchingXmlSchemaAsString { get; } = @"<?xml version=""1.0"" encoding=""utf-8""?>
            <xs:schema xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"" attributeFormDefault=""unqualified"" elementFormDefault=""qualified"">
              <xsd:element name=""Location"">
                <xsd:complexType>
                  <xsd:sequence>
                    <xsd:element name=""Country"" type=""xsd:string"" />
                    <xsd:element name=""Province"" type=""xsd:string"" />
                    <xsd:element name=""ZipCode"" type=""xsd:unsignedInt"" />
                    <xsd:element name=""Places"">
                      <xsd:complexType>
                        <xsd:sequence>
                          <xsd:element maxOccurs=""unbounded"" name=""Place"">
                            <xsd:complexType>
                              <xsd:sequence>
                                <xsd:element name=""Name"" type=""xsd:string"" />
                                <xsd:element name=""Inhabitants"" type=""xsd:unsignedInt"" />
                                <xsd:element name=""IsCapital"" type=""xsd:boolean"" />
                              </xsd:sequence>
                            </xsd:complexType>
                          </xsd:element>
                        </xsd:sequence>
                      </xsd:complexType>
                    </xsd:element>
                  </xsd:sequence>
                </xsd:complexType>
              </xsd:element>
            </xs:schema>";

        /// <summary>
        /// An XML schema that is not correctly formatted.
        /// </summary>
        internal static string InvalidXmlSchemaAsString { get; } = @"<?xml version=""1.0"" encoding=""utf-8""?>
            <xs:schema xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"" attributeFormDefault=""unqualified"" elementFormDefault=""qualified"">
              <xsd:banana name=""Location"">
                <xsd:complexType>
                  <xsd:sequence>
                    <xsd:element name=""Country"" type=""xsd:string"" />
                    <xsd:element name=""Province"" type=""xsd:string"" />
                    <xsd:element name=""ZipCode"" type=""xsd:unsignedInt"" />
                    <xsd:element name=""Places"">
                      <xsd:complexType>
                        <xsd:sequence>
                          <xsd:element maxOccurs=""unbounded"" name=""Place"">
                            <xsd:complexType>
                              <xsd:sequence>
                                <xsd:element name=""Name"" type=""xsd:string"" />
                                <xsd:element name=""Inhabitants"" type=""xsd:unsignedInt"" />
                                <xsd:element name=""IsCapital"" type=""xsd:boolean"" />
                              </xsd:sequence>
                            </xsd:complexType>
                          </xsd:element>
                        </xsd:sequence>
                      </xsd:complexType>
                    </xsd:element>
                  </xsd:sequence>
                </xsd:complexType>
              </xsd:banana>
            </xs:schema>";

        /// <summary>
        /// A sample XML response matching the inline DTD.
        /// </summary>
        internal static string XmlWithMatchingInlineDtd { get; } = @"<?xml version=""1.0"" encoding=""utf-8""?>
            <!DOCTYPE store [
              <!ELEMENT store (item)*> 
              <!ELEMENT item (name,dept,price)>
              <!ATTLIST item type CDATA #REQUIRED>
              <!ATTLIST item ISBN CDATA #REQUIRED>
              <!ELEMENT name (#PCDATA)>
              <!ELEMENT dept (#PCDATA)>
              <!ELEMENT price (#PCDATA)>]>
            <store>
              <item type=""supplies""  ISBN=""2-3631-4"">
                <name>paint</name>
                <dept>interior design</dept>
                <price>16.95</price>
              </item>
            </store>";

        /// <summary>
        /// A sample XML response not matching the inline DTD.
        /// </summary>
        internal static string XmlWithNonMatchingInlineDtd { get; } = @"<?xml version=""1.0"" encoding=""utf-8""?>
            <!DOCTYPE store [
              <!ELEMENT store (item)*> 
              <!ELEMENT item (name,dept,price)>
              <!ATTLIST item type CDATA #REQUIRED>
              <!ATTLIST item ISBN CDATA #REQUIRED>
              <!ELEMENT name (#PCDATA)>
              <!ELEMENT price (#PCDATA)>]>
            <store>
              <item type=""supplies""  ISBN=""2-3631-4"">
                <name>paint</name>
                <dept>interior design</dept>
                <price>16.95</price>
              </item>
            </store>";
    }
}
