import { DoctorAvailability } from "./DoctorAvailability";
import { DoctorQualification } from "./DoctorQualification";

export interface Doctor {
  id: string;
  fullName: string;
  yearsOfExperience?: number;
  specialization?: string;
  licenseNumber?: string;
  consultationFee?: number;
  workplaceType?: WorkplaceType;
  qualifications: DoctorQualification[];
  availabilities: DoctorAvailability[];
}

export enum WorkplaceType {
  Clinic = 'Clinic',
  Hospital = 'Hospital',
  Telemedicine = 'Telemedicine'
}
