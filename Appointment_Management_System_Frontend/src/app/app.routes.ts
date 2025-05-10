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
import { DoctorsComponent } from './doctors/doctors.component';
import { AddDoctorComponent } from './doctors/add-doctor/add-doctor.component';
import { UpdateDoctorComponent } from './doctors/update-doctor/update-doctor.component';
import { DoctorDetailsComponent } from './doctors/doctor-details/doctor-details.component';
import { PatientDetailsComponent } from './patients/patient-details/patient-details.component';
import { ResetPasswordComponent } from './login/reset-password/reset-password.component';
import { ForgotPasswordComponent } from './login/forgot-password/forgot-password.component';

export const routes: Routes = [
    { path: '', redirectTo: '/appointments', pathMatch: 'full' },

    // Authentication
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'reset-password', component: ResetPasswordComponent },
    { path: 'forgot-password', component: ForgotPasswordComponent },
  
    // Appointments (Accessible to All Authenticated Users)
    { path: 'appointments', component: AppointmentsComponent, canActivate: [authGuard] },
    { path: 'add-appointment', component: AddAppointmentComponent },
    { path: 'update-appointment', component: UpdateAppointmentComponent, canActivate: [authGuard], data: { role: ['Patient', 'Admin'] } },
    { path: 'delete-appointment', component: CancelAppointmentComponent, canActivate: [authGuard] },
    { path: 'update-appointment-status', component: UpdateAppointmentStatusComponent, canActivate: [authGuard], data: { role: ['Doctor'] } },
  
    // Patients 
    { path: 'patients', component: PatientsComponent, canActivate: [authGuard], data: { role: ['Admin'] } },
    { path: 'patient-details/:id', component: PatientDetailsComponent, canActivate: [authGuard], data: { role: ['Admin'] } },
  
    // Doctors 
    { path: 'doctors', component: DoctorsComponent },
    { path: 'doctor-details/:id', component: DoctorDetailsComponent},
    //(Admin Only)
    { path: 'add-doctor', component: AddDoctorComponent, canActivate: [authGuard], data: { role: ['Admin'] } },
    { path: 'update-doctor', component: UpdateDoctorComponent, canActivate: [authGuard], data: { role: ['Admin'] } },
  
    { path: '**', redirectTo: '/appointments' }
];
