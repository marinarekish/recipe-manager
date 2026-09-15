import { Component, inject, OnInit } from '@angular/core';
import { AdminLookupService} from '../../data/admin-lookup.service';
import { ActiveTab, CategoryResponse, CuisineResponse, IngredientResponse } from '../../data/admin-lookup.models';
import { NotificationService } from '../../../../core/ui/notification.service';

@Component({
  selector: 'app-admin-lookups',
  standalone: true,
  imports: [],
  templateUrl: './admin-lookups.component.html',
  styleUrl: './admin-lookups.component.scss',
})
export class AdminLookupsComponent implements OnInit {
  readonly ActiveTab = ActiveTab;
  private readonly adminLookupService = inject(AdminLookupService);
  private readonly notify = inject(NotificationService);

  categories: CategoryResponse[] = [];
  cuisines: CuisineResponse[] = [];
  ingredients: IngredientResponse[] = [];

  loading = true;
  errorMessage = '';

  currentTab = ActiveTab.categories; // default

  ngOnInit(): void {
    this.loadTab(this.currentTab);
  }

  loadTab(tab: ActiveTab) {
    this.loading = true;
    this.errorMessage = '';

    if (tab === ActiveTab.categories) {
      this.adminLookupService.getCategories().subscribe({
        next: (categories) => {
          this.categories = (categories ?? [])
            .slice()
            .sort((a, b) => a.name.localeCompare(b.name, undefined, { sensitivity: 'base' }));
          this.loading = false;
        },
        error: () => {
          this.loading = false;
          this.errorMessage = 'Could not load categories. Please try again.';
        }
      })
    } else if (tab === ActiveTab.cuisines) {
      this.adminLookupService.getCuisines().subscribe({
        next: (cuisines) => {
          this.cuisines = (cuisines ?? [])
            .slice()
            .sort((a, b) => a.name.localeCompare(b.name, undefined, { sensitivity: 'base' }));
          this.loading = false;
        },
        error: () => {
          this.loading = false;
          this.errorMessage = 'Could not load cuisines. Please try again.';
        }
      })
    } else if (tab === ActiveTab.ingredients) {
      this.adminLookupService.getIngredients().subscribe({
        next: (ingredients) => {
          this.ingredients = (ingredients ?? [])
            .slice()
            .sort((a, b) => a.name.localeCompare(b.name, undefined, { sensitivity: 'base' }));
          this.loading = false;
        },
        error: () => {
          this.loading = false;
          this.errorMessage = 'Could not load ingredients. Please try again.';
        }
      })
    }
  }

  switchTab(tab: ActiveTab) {
    this.currentTab = tab;

    this.loadTab(tab);
  }


  deleteItem(index: number) {
    if (!window.confirm('Are you sure?')) {
      return;
    }

    if (this.currentTab === ActiveTab.categories) {
      this.adminLookupService.deleteCategory(index).subscribe({
        next: (categories) => {
          this.categories = this.categories.filter(c => c.categoryId !== index);
          this.notify.success("Category deleted successfully.");
        },
        error: () => {
          this.errorMessage = 'Something went wrong';
          this.notify.error('Could not delete the category.');
        }
      });
    } else if (this.currentTab === ActiveTab.cuisines) {
      this.adminLookupService.deleteCuisine(index).subscribe({
        next: (cuisines) => {
          this.cuisines = this.cuisines.filter(c => c.cuisineId !== index);
          this.notify.success("Cuisine deleted successfully.");
        },
        error: () => {
          this.errorMessage = 'Something went wrong';
          this.notify.error('Could not delete the cuisine.');
        }
      });
    } else if (this.currentTab === ActiveTab.ingredients) {
      this.adminLookupService.deleteIngredient(index).subscribe({
        next: (ingredients) => {
          this.ingredients = this.ingredients.filter(i => i.ingredientId !== index);
          this.notify.success("Ingredient deleted successfully.");
        },
        error: () => {
          this.errorMessage = 'Something went wrong';
          this.notify.error('Could not delete the ingredient.');
        }
      });
    }
  }
}
