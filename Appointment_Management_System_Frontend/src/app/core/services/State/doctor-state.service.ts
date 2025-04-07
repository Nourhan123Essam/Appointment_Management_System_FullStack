import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of, tap } from 'rxjs';
import { Doctor } from '../../Interfaces/Doctor';
import { DoctorApiService } from '../Api/doctor-api.service';
import { DoctorAvailability } from '../../Interfaces/DoctorAvailability';
import { DoctorQualification } from '../../Interfaces/DoctorQualification';

@Injectable({
  providedIn: 'root'
})
export class DoctorStateService {

  private doctorsSubject = new BehaviorSubject<Doctor[] | null>(null);
  doctors$ = this.doctorsSubject.asObservable();

  private doctorQualificationsSubject = new BehaviorSubject<{ [doctorId: string]: DoctorQualification[] }>({});
  doctorQualifications$ = this.doctorQualificationsSubject.asObservable();

  private doctorAvailabilitiesSubject = new BehaviorSubject<{ [doctorId: string]: DoctorAvailability[] }>({});
  doctorAvailabilities$ = this.doctorAvailabilitiesSubject.asObservable();

  constructor(private doctorApi: DoctorApiService) {}
 
  ///////////////////////////////////////////////////
  // Fetch functions
  ///////////////////////////////////////////////////

  // Lazy load doctors → Fetch only if not already loaded
  fetchDoctors(): Observable<Doctor[]> {
    if (this.doctorsSubject.value) {
      return of(this.doctorsSubject.value);
    }
    return this.doctorApi.fetchDoctors().pipe(
      tap(doctors => this.doctorsSubject.next(doctors))
    );
  }

  // Get a specific doctor by ID (without fetching)
  getDoctorById(doctorId: string): Doctor | undefined {
    return this.doctorsSubject.value?.find(doc => doc.id === doctorId);
  }

  // Fetch doctor availabilities only if not cached
  getDoctorAvailabilities(doctorId: string): Observable<DoctorAvailability[]> {
    if (this.doctorAvailabilitiesSubject.value[doctorId]) {
      return of(this.doctorAvailabilitiesSubject.value[doctorId]);
    }
    return this.doctorApi.fetchDoctorAvailabilities(doctorId).pipe(
      tap(availabilities => {
        const updatedData = { ...this.doctorAvailabilitiesSubject.value, [doctorId]: availabilities };
        this.doctorAvailabilitiesSubject.next(updatedData);
      })
    );
  }

  // Fetch doctor qualifications only if not cached
  getDoctorQualifications(doctorId: string): Observable<DoctorQualification[]> {
    if (this.doctorQualificationsSubject.value[doctorId]) {
      return of(this.doctorQualificationsSubject.value[doctorId]);
    }
    return this.doctorApi.fetchDoctorQualifications(doctorId).pipe(
      tap(qualifications => {
        const updatedData = { ...this.doctorQualificationsSubject.value, [doctorId]: qualifications };
        this.doctorQualificationsSubject.next(updatedData);
      })
    );
  }

  ///////////////////////////////////////////////////
  // Add functions
  ///////////////////////////////////////////////////

  // Add new doctor → Update state only if API succeeds
  addDoctor(newDoctor: Doctor): Observable<Doctor> {
    return this.doctorApi.addDoctor(newDoctor).pipe(
      tap((savedDoctor) => {
        console.log("new created doctor", savedDoctor);
        
        const updatedDoctors = [...(this.doctorsSubject.value || []), savedDoctor];
        console.log("updated list of doctor", updatedDoctors);
        
        this.doctorsSubject.next(updatedDoctors);
      })
    );
  }

 // Add new doctor Availability → Update state only if API succeeds
  addDoctorAvailability(doctorId: string, availability: DoctorAvailability): Observable<DoctorAvailability> {
    return this.doctorApi.addDoctorAvailability(availability).pipe(
      tap((newAvailability) => {
        const updatedAvailabilities = { ...this.doctorAvailabilitiesSubject.value };

        if (!updatedAvailabilities[doctorId]) {
          updatedAvailabilities[doctorId] = [];
        }

        updatedAvailabilities[doctorId] = [...updatedAvailabilities[doctorId], newAvailability];
        this.doctorAvailabilitiesSubject.next(updatedAvailabilities);
      })
    );
  }

 
  // Add new doctor qualification → Update state only if API succeeds
  addDoctorQualification(doctorId: string, qualification: DoctorQualification): Observable<DoctorQualification> {
    return this.doctorApi.addDoctorQualification(qualification).pipe(
      tap((newQualification) => {
        const updatedQualifications = { ...this.doctorQualificationsSubject.value };

        if (!updatedQualifications[doctorId]) {
          updatedQualifications[doctorId] = [];
        }

        updatedQualifications[doctorId] = [...updatedQualifications[doctorId], newQualification];
        this.doctorQualificationsSubject.next(updatedQualifications);
      })
    );
  }



  ///////////////////////////////////////////////////
  // Update functions
  ///////////////////////////////////////////////////

  // Update doctor details → Update state only if API succeeds
  updateDoctor(updatedDoctor: Doctor): Observable<string> {
    return this.doctorApi.updateDoctor(updatedDoctor.id, updatedDoctor).pipe(
      tap((savedDoctor) => {
        debugger;
        const updatedDoctors = this.doctorsSubject.value?.map(doc =>
          doc.id === updatedDoctor.id ? updatedDoctor : doc
        ) || [];
        this.doctorsSubject.next(updatedDoctors);
      })
    );
  }

  // Update doctor availabilities → Update state only if API succeeds
  updateDoctorAvailability(doctorId: string, availability: DoctorAvailability): Observable<any> {
    return this.doctorApi.updateDoctorAvailability(availability.id, availability).pipe(
      tap(() => {
        const updatedAvailabilities = { ...this.doctorAvailabilitiesSubject.value };
  
        if (updatedAvailabilities[doctorId]) {
          updatedAvailabilities[doctorId] = updatedAvailabilities[doctorId].map(avail =>
            avail.id === availability.id ? availability : avail
          );
        }
  
        this.doctorAvailabilitiesSubject.next(updatedAvailabilities);
      })
    );
  }
  

  // Update doctor qualifications → Update state only if API succeeds
  updateDoctorQualifications(doctorId: string, qualification: DoctorQualification): Observable<any> {
    return this.doctorApi.updateDoctorQualification(qualification.id, qualification).pipe(
      tap(() => {
        const updatedQualifications = { ...this.doctorQualificationsSubject.value };
  
        if (updatedQualifications[doctorId]) {
          updatedQualifications[doctorId] = updatedQualifications[doctorId].map(qual =>
            qual.id === qualification.id ? qualification : qual
          );
        }
  
        this.doctorQualificationsSubject.next(updatedQualifications);
      })
    );
  }
  

  ///////////////////////////////////////////////////
  // Delete functions
  ///////////////////////////////////////////////////

  // Delete doctor → Remove from state only if API succeeds
  deleteDoctor(doctorId: string): Observable<string> {
    return this.doctorApi.deleteDoctor(doctorId).pipe(
      tap(() => {
        const updatedDoctors = this.doctorsSubject.value?.filter(doc => doc.id !== doctorId) || [];
        this.doctorsSubject.next(updatedDoctors);
      })
    );
  }

  // Delete doctor availabilities → Remove from state only if API succeeds
  deleteDoctorAvailability(doctorId: string, availabilityId: number): Observable<any> {
    return this.doctorApi.deleteDoctorAvailability(availabilityId).pipe(
      tap(() => {
        const updatedAvailabilities = { ...this.doctorAvailabilitiesSubject.value };
  
        if (updatedAvailabilities[doctorId]) {
          updatedAvailabilities[doctorId] = updatedAvailabilities[doctorId].filter(
            availability => availability.id !== availabilityId
          );
        }
  
        this.doctorAvailabilitiesSubject.next(updatedAvailabilities);
      })
    );
  }
  

  // Delete doctor qualifications → Remove from state only if API succeeds
  deleteDoctorQualification(doctorId: string, qualificationId: number): Observable<any> {
    return this.doctorApi.deleteDoctorQualification(qualificationId).pipe(
      tap(() => {
        const updatedQualifications = { ...this.doctorQualificationsSubject.value };
  
        if (updatedQualifications[doctorId]) {
          updatedQualifications[doctorId] = updatedQualifications[doctorId].filter(
            qualification => qualification.id !== qualificationId
          );
        }
  
        this.doctorQualificationsSubject.next(updatedQualifications);
      })
    );
  }
  
}
