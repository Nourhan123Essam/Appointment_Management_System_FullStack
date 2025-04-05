import { DoctorAvailability } from "./DoctorAvailability";
import { DoctorQualification } from "./DoctorQualification";

export interface Doctor {
  id: string;
  fullName: string;
  email: string;
  password: string;
  yearsOfExperience?: number;
  specialization?: string;
  licenseNumber?: string;
  consultationFee?: number;
  workplaceType?: WorkplaceType;
  TotalRatingScore?: number;
  TotalRatingsGiven?: number;
  qualifications: DoctorQualification[];
  availabilities: DoctorAvailability[];

}

export enum WorkplaceType {
  Online,
  Hospital,
  Clinic
}

