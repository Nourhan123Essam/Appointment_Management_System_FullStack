export interface User {
    Id: string;
    firstName: string;
    lastName: string;
    email: string;
    phone: string;
    dateOfBirth: Date;
    address?: string;
    gender: string;
    password: string;
  }