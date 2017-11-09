import { TrackableEntity, TrackableSet } from 'trackable-entities';
import { Category } from './category';
import { OrderDetail } from './orderdetail';

export class Product extends TrackableEntity {

  productId: number;
  productName: string;
  categoryId: number | null;
  unitPrice: number | null;
  discontinued: boolean;
  rowVersion: number[];
  category: Category;
  orderDetails: TrackableSet<OrderDetail>;

  constructor() {
    super();
    return super.proxify(this);
  }
}
