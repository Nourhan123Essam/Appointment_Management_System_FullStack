  export interface DoctorAvailability {
    id: number;
    dayOfWeek: DayOfWeek;
    startTime: string; // Using string because TimeSpan is not a native JS type
    endTime: string;
    doctorId: string;
  }
  
  export enum DayOfWeek {
    Sunday,
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday
  }
  