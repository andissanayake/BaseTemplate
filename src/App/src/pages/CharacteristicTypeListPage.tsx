import { Card } from "antd";
import { CharacteristicTypeList } from "../features/CharacteristicType/CharacteristicTypeList";

const CharacteristicTypeListPage: React.FC = () => {
  return (
    <Card>
      <CharacteristicTypeList />
    </Card>
  );
};

export { CharacteristicTypeListPage };
