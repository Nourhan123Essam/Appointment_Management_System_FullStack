import { Component } from '@angular/core';
import { AppWhyBookSectionComponent } from "./why-book-section/why-book-section.component";
import { HomeFaqComponent } from "./home-faq/home-faq.component";
import { HomeHeroStatsComponent } from "./home-hero-stats/home-hero-stats.component";
import { SpecializationsCarouselComponent } from "./specializations-carousel/specializations-carousel.component";
import { TopDoctorsListComponent } from "./top-doctors-list/top-doctors-list.component";

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [AppWhyBookSectionComponent, HomeFaqComponent, HomeHeroStatsComponent, SpecializationsCarouselComponent, TopDoctorsListComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {

}
