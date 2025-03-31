import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './login/register/register.component';
import { authGuard } from './core/guards/auth.guard';
import { AppointmentsComponent } from './appointments/appointments.component';
import { AddAppointmentComponent } from './appointments/add-appointment/add-appointment.component';
import { UpdateAppointmentComponent } from './appointments/update-appointment/update-appointment.component';
import { UpdateAppointmentStatusComponent } from './appointments/update-appointment-status/update-appointment-status.component';
import { CancelAppointmentComponent } from './appointments/cancel-appointment/cancel-appointment.component';
import { PatientsComponent } from './patients/patients.component';
import { AddPatientComponent } from './patients/add-patient/add-patient.component';
import { DeletePatientComponent } from './patients/delete-patient/delete-patient.component';
import { DoctorsComponent } from './doctors/doctors.component';
import { AddDoctorComponent } from './doctors/add-doctor/add-doctor.component';
import { UpdateDoctorComponent } from './doctors/update-doctor/update-doctor.component';
import { DoctorDetailsComponent } from './doctors/doctor-details/doctor-details.component';

export const routes: Routes = [
    { path: '', redirectTo: '/appointments', pathMatch: 'full' },

    // Authentication
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
  
    // Appointments (Accessible to All Authenticated Users)
    { path: 'appointments', component: AppointmentsComponent, canActivate: [authGuard] },
    { path: 'add-appointment', component: AddAppointmentComponent },
    { path: 'update-appointment', component: UpdateAppointmentComponent, canActivate: [authGuard], data: { role: ['Patient', 'Admin'] } },
    { path: 'delete-appointment', component: CancelAppointmentComponent, canActivate: [authGuard] },
    { path: 'update-appointment-status', component: UpdateAppointmentStatusComponent, canActivate: [authGuard], data: { role: ['Doctor'] } },
  
    // Patients (Admin Only)
    { path: 'patients', component: PatientsComponent, canActivate: [authGuard], data: { role: ['Admin'] } },
    { path: 'add-patient', component: AddPatientComponent, canActivate: [authGuard], data: { role: ['Admin'] } },
    { path: 'delete-patient', component: DeletePatientComponent, canActivate: [authGuard], data: { role: ['Admin'] } },
  
    // Doctors (Admin Only)
    { path: 'doctors', component: DoctorsComponent },
    { path: 'add-doctor', component: AddDoctorComponent, canActivate: [authGuard], data: { role: ['Admin'] } },
    { path: 'update-doctor', component: UpdateDoctorComponent, canActivate: [authGuard], data: { role: ['Admin'] } },
    { path: 'doctor-details/:id', component: DoctorDetailsComponent, canActivate: [authGuard], data: { role: ['Admin'] } },
  
    { path: '**', redirectTo: '/appointments' }
];
