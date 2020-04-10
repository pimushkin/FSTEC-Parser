using System;

namespace FSTEC_Parser
{
    public class Risk : ICloneable
    {
        private sbyte _violationOfConfidentiality = -1;
        private sbyte _integrityViolation = -1;
        private sbyte _violationOfAvailability = -1;
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SourceOfThreat { get; set; }
        public string InteractionObject { get; set; }
        public string ViolationOfConfidentiality 
        {
            get
            {
                if (_violationOfConfidentiality == 1)
                {
                    return "да";
                }
                return _violationOfConfidentiality == 0 ? "нет" : "";
            }
                set
            {
                if (value == "0" || value == "1")
                {
                    _violationOfConfidentiality = Convert.ToSByte(value);
                }
                else
                {
                    _violationOfConfidentiality = -1;
                }
            } 
        }
        public string IntegrityViolation
        {
            get
            {
                if (_integrityViolation == 1)
                {
                    return "да";
                }
                return _integrityViolation == 0 ? "нет" : "";
            }
            set
            {
                if (value == "0" || value == "1")
                {
                    _integrityViolation = Convert.ToSByte(value);
                }
                else
                {
                    _integrityViolation = -1;
                }
            }
        }
        public string ViolationOfAvailability
        {
            get
            {
                if (_violationOfAvailability == 1)
                {
                    return "да";
                }
                return _violationOfAvailability == 0 ? "нет" : "";
            }
            set
            {
                if (value == "0" || value == "1")
                {
                    _violationOfAvailability = Convert.ToSByte(value);
                }
                else
                {
                    _violationOfAvailability = -1;
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            Risk risk = obj as Risk;
            if (risk as Risk == null)
                return false;
            return risk.ID.Equals(ID) &&
                   risk.Name.Equals(Name) &&
                   risk.Description.Equals(Description) &&
                   risk.SourceOfThreat.Equals(SourceOfThreat) &&
                   risk.InteractionObject.Equals(InteractionObject) &&
                   risk._violationOfConfidentiality.Equals(_violationOfConfidentiality) &&
                   risk._integrityViolation.Equals(_integrityViolation) &&
                   risk._violationOfAvailability.Equals(_violationOfAvailability);
        }

        public static bool operator ==(Risk risk1, Risk risk2)
        {
            return Equals(risk1, risk2);
        }

        public static bool operator !=(Risk risk1, Risk risk2)
        {
            return !Equals(risk1, risk2);
        }

        public object Clone()
        {
            return new Risk() 
            {
                ID = ID, 
                Name = Name, 
                Description = Description, 
                SourceOfThreat = SourceOfThreat, 
                InteractionObject = InteractionObject, 
                _violationOfConfidentiality = _violationOfConfidentiality, 
                _integrityViolation = _integrityViolation, 
                _violationOfAvailability = _violationOfAvailability };
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _violationOfConfidentiality.GetHashCode();
                hashCode = (hashCode * 397) ^ _integrityViolation.GetHashCode();
                hashCode = (hashCode * 397) ^ _violationOfAvailability.GetHashCode();
                hashCode = (hashCode * 397) ^ (ID != null ? ID.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (SourceOfThreat != null ? SourceOfThreat.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (InteractionObject != null ? InteractionObject.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}