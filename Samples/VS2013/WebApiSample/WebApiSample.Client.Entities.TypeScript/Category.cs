import { TrackableEntity, TrackableSet } from 'trackable-entities';
import { Product } from './product';

export class Category extends TrackableEntity {

  categoryId: number;
  categoryName: string;
  products: TrackableSet<Product>;

  constructor() {
    super();
    return super.proxify(this);
  }
}
