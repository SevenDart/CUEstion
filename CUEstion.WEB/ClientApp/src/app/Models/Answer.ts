import {AppComment} from './AppComment';

export interface Answer {
  id: number;
  text: string;
  rate: number;
  createTime: Date;
  updateTime: Date;
  comments: AppComment[];
}
