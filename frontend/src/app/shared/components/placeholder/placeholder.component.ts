import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-placeholder',
  template: `
    <h2>{{ title }}</h2>
    <p>Coming soon.</p>
  `,
})
export class PlaceholderComponent implements OnInit {
  title = '';

  constructor(private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.title = this.route.snapshot.data['title'] ?? 'Page';
  }
}
